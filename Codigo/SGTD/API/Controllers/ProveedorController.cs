using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Service.Implementations;
using Shared.DTOs.ProveedorDTOs;
using Shared.Entidades;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;

        public ProveedorController(IProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearProveedor([FromBody] ProveedorCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Los datos del cliente no pueden ser nulos.");

            try
            {
                var proveedor = await _proveedorService.CrearAsync(dto);
                if (proveedor != null)
                    return CreatedAtAction(nameof(ObtenerProveedorPorId), new { id = proveedor.Id }, proveedor);

                return Conflict("Ya existe un proveedor con esos datos.");

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error interno al crear el proveedor.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProveedor()
        {
            try
            {
                var proveedor = await _proveedorService.ObtenerTodosAsync();
                if (proveedor == null || !proveedor.Any())
                    return NoContent();

                return Ok(proveedor);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al obtener los proveedor.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerProveedorPorId(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a cero.");

            try
            {
                var proveedor = await _proveedorService.ObtenerPorIdAsync(id);
                if (proveedor != null)

                    return Ok(proveedor);
                return BadRequest(proveedor);

            }
            catch (Exception)
            {
                return NotFound($"No se encontró un proveedor con ID {id}.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProveedor(int id, [FromBody] ProveedorUpdateDTO dto)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a cero.");
            if (dto == null)
                return BadRequest("Los datos de actualización no pueden ser nulos.");

            try
            {
                var proveedor = await _proveedorService.Editar(id, dto);
                if (proveedor != null)
                    return Ok(proveedor);


                return BadRequest(proveedor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al actualizar el proveedor.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProveedor(int id)
        {
            try
            {
                await _proveedorService.Eliminar(id);
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
                return StatusCode(500, "Ocurrio un error interno al eliminar el proveedor");
            }
        }

        [HttpGet("activos")]
        public async Task<IActionResult> ObtenerProveedoresActivos()
        {
            try
            {
                var proveedoresActivos = await _proveedorService.ObtenerProveedoresActivosAsync();
                if (proveedoresActivos == null || !proveedoresActivos.Any())
                    return NoContent();

                return Ok(proveedoresActivos);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al obtener los proveedores activos.");
            }
        }
    }
}
