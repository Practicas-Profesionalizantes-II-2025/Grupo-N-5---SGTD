using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Service.Implementations;
using Shared.DTOs.FacturaDTOs;
using Shared.DTOs.ProductoDTOs;
using System.Reflection.Metadata;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturaController : ControllerBase
    {
        private IFacturaService _facturaService;

        public FacturaController (IFacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearFactura([FromBody] FacturaCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Los datos del cliente no pueden ser nulos.");

            try
            {
                var factura = await _facturaService.CrearAsync(dto);
                if (factura != null)
                    return CreatedAtAction(nameof(ObtenerFacturaPorId), new { id = factura.Id }, factura);
               
                return Conflict("Ya existe una factura con esos datos.");
                
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error interno al crear la factura.");
            }

        }


        [HttpGet]
        public async Task<IActionResult> ObtenerFactura()
        {
            try
            {
                var facturas = await _facturaService.ObtenerTodosAsync();
                if (facturas == null || !facturas.Any())
                    return NoContent();
                return Ok(facturas);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error interno al obtener las facturas.");
            }

        }
           

            [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerFacturaPorId(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a cero.");
            try
            {
                var factura = await _facturaService.ObtenerPorIdAsync(id);
                if (factura != null)
                    return Ok(factura);
                return BadRequest(factura);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception)
            {
                return NotFound($"No se encontró un cliente con ID {id}.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarFactura(int id)
        {
            try
            {
                await _facturaService.Eliminar(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // 404 si no encuentra nada el hdp
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // 400 si el id es invalido
            }
            catch (Exception ex)
            {
                // log para ver que error tengo jajan´t :(
                return StatusCode(500, "Ocurrio un error interno al eliminar la factura");
            }

        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DescargarFacturaPdf(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a cero.");

            try
            {
                var factura = await _facturaService.ObtenerPorIdAsync(id);
                if (factura == null)
                    return NotFound($"No se encontró ninguna factura con ID {id}.");

                // Generar PDF con QuestPDF
                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);

                        page.Header().Text($"Factura #{factura.Id}")
                            .FontSize(20).Bold().AlignCenter();

                        page.Content().Stack(stack =>
                        {
                            stack.Item().Text($"Cliente: {factura.RazonSocial}");
                            stack.Item().Text($"Fecha: {factura.FechaEmision:dd/MM/yyyy}");
                            stack.Item().Text($"Dirección Fiscal: {factura.DireccionFiscal}");

                            stack.Item().LineHorizontal(1);

                            stack.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(4); // Producto
                                    c.RelativeColumn(2); // Cantidad
                                    c.RelativeColumn(2); // Precio Unitario
                                    c.RelativeColumn(2); // Subtotal
                                });

                                table.Header(h =>
                                {
                                    h.Cell().Text("Producto").Bold();
                                    h.Cell().Text("Cantidad").Bold();
                                    h.Cell().Text("Precio Unitario").Bold();
                                    h.Cell().Text("Subtotal").Bold();
                                });

                                foreach (var fp in factura.Productos)
                                {
                                    table.Cell().Text(fp.Nombre);
                                    table.Cell().Text(fp.Cantidad.ToString());
                                    table.Cell().Text($"${fp.PrecioUnitario}");
                                    table.Cell().Text($"${fp.Subtotal}");
                                }
                            });

                            stack.Item().PaddingTop(20).AlignRight()
                                .Text($"Total: ${factura.Productos.Sum(p => p.Cantidad * p.PrecioUnitario)}").Bold();
                        });
                    });
                });

                var pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", $"Factura_{factura.Id}.pdf");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al generar el PDF de la factura.");
            }
        }

    }
}
