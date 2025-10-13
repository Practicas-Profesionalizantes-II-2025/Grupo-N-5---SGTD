using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Models.DTOs.FacturaDto;
using MVC.Models.DTOs.ProductoDto;
using MVC.Models.DTOs.DisciplinaDto;
using MVC.Models.DTOs.ClienteDto;
using System.Text.Json;
using Shared.DTOs.FacturaDTOs;
using System.Security.Claims;

namespace MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "factura"; // endpoint base de tu API

        public ReportesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ReportesApi");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiBaseUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var factura = JsonSerializer.Deserialize<List<FacturaReadDTO>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return View(factura);
                }
            }
            catch
            {
                ViewBag.Error = "Error al cargar los Reportes";
            }

            return View(new List<FacturaReadDTO>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarDatosSelects();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FacturaCreateDTO factura)
        {
            ModelState.Remove(nameof(FacturaCreateDTO.UsuarioId));
            factura.UsuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (!string.IsNullOrWhiteSpace(factura.ProductosJson))
            {
                factura.Productos = System.Text.Json.JsonSerializer.Deserialize<
                    List<FacturaProductoCreateDTO>>(
                        factura.ProductosJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Deserializar manualmente el JSON a la lista real
                    if (!string.IsNullOrWhiteSpace(factura.ProductosJson))
                    {
                        factura.Productos = System.Text.Json.JsonSerializer.Deserialize<
                            List<FacturaProductoCreateDTO>>(factura.ProductosJson,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }

                    // Fecha actual si no se envió
                    if (factura.FechaEmision == default)
                        factura.FechaEmision = DateTime.Now;

                    var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, factura);

                    if (response.IsSuccessStatusCode)
                        return RedirectToAction(nameof(Index));

                    ModelState.AddModelError("", "Error al crear la factura: " + response.ReasonPhrase);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error de conexión con la API: " + ex.Message);
                }
            }

            await CargarDatosSelects();
            return View(factura);
        }

        // Método privado para cargar disciplinas, productos y clientes
        private async Task CargarDatosSelects()
        {
            // Disciplinas
            var responseDisciplinas = await _httpClient.GetAsync("disciplina");
            if (responseDisciplinas.IsSuccessStatusCode)
            {
                var content = await responseDisciplinas.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    ViewBag.Disciplinas = JsonSerializer.Deserialize<IEnumerable<DisciplinaReadDTO>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }

            // Productos
            var responseProductos = await _httpClient.GetAsync("producto/activos");
            if (responseProductos.IsSuccessStatusCode)
            {
                var content = await responseProductos.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    ViewBag.Productos = JsonSerializer.Deserialize<IEnumerable<ProductoReadDTO>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }

            // Clientes
            var responseClientes = await _httpClient.GetAsync("cliente");
            if (responseClientes.IsSuccessStatusCode)
            {
                var content = await responseClientes.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    ViewBag.Clientes = JsonSerializer.Deserialize<IEnumerable<ClienteReadDTO>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
        }

        // Endpoint para filtrar productos por disciplina (AJAX)
        [HttpGet]
        public async Task<IActionResult> ProductosPorDisciplina(int disciplinaId)
        {
            var response = await _httpClient.GetAsync($"producto/filtrarPorDisciplina/{disciplinaId}");
            if (!response.IsSuccessStatusCode)
                return Json(new List<ProductoReadDTO>());

            var content = await response.Content.ReadAsStringAsync();
            var productos = JsonSerializer.Deserialize<IEnumerable<ProductoReadDTO>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Json(productos);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al eliminar la factura");
            }
        }


        public async Task<IActionResult> DescargarPdf(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}/pdf");

                if (response.IsSuccessStatusCode)
                {
                    var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                    var fileName = $"Factura_{id}.pdf";

                    return File(pdfBytes, "application/pdf", fileName);
                }

                return NotFound("No se encontró la factura para generar el PDF");
            }
            catch (Exception)
            {
                return StatusCode(500, "Error al descargar el PDF de la factura");
            }
        }

        public async Task<IActionResult> Preview(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}/pdf");

                if (!response.IsSuccessStatusCode)
                    return NotFound("No se encontró la factura para generar el PDF");

                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                string base64Pdf = Convert.ToBase64String(pdfBytes);
                ViewBag.PdfBase64 = base64Pdf;

                return View();
            }
            catch (Exception)
            {
                return StatusCode(500, "Error al mostrar la vista previa del PDF");
            }
        }

    }
}

