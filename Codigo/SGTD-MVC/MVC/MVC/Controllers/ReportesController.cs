using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Models.DTOs.FacturaDto;
using MVC.Models.DTOs.ProductoDto;
using MVC.Models.Entity;
using System.Text.Json;

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
            catch (Exception ex)
            {
                // Log error si es necesario
                ViewBag.Error = "Error al cargar los Reportes";
            }

            return View(new List<FacturaReadDTO>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FacturaCreateDTO factura)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, factura);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError("", "Error al crear la factura");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error de conexión con la API");
                }
            }
            return View(factura);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var factura = JsonSerializer.Deserialize<Factura>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return View(factura);
                }
            }
            catch (Exception ex)
            {
                // Log error
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, FacturaUpdateDTO factura)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{id}", factura);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError("", "Error al actualizar la factura");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error de conexión con la API");
                }
            }
            return View(factura);
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
    }
}

