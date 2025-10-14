using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Models.DTOs.ClienteDto;
using System.Text.Json;

namespace MVC.Controllers
{
    [Authorize(Roles = "Admin,Empleado")]
    public class ClientesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "Cliente"; // endpoint base de tu API

        public ClientesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ClientesApi");
        }

        // LISTADO DE CLIENTES
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiBaseUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return View(new List<ClienteReadDTO>());

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var clientes = JsonSerializer.Deserialize<List<ClienteReadDTO>>(content,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        return View(clientes);
                    }
                }
                return View(new List<ClienteReadDTO>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar los clientes.";
                return View(new List<ClienteReadDTO>());
            }
        }

        // FORMULARIO CREATE
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarProvinciasAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClienteCreateDTO cliente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, cliente);
                    if (response.IsSuccessStatusCode)
                        return RedirectToAction(nameof(Index));

                    ModelState.AddModelError("", "Error al crear el cliente.");
                }
                catch
                {
                    ModelState.AddModelError("", "Error de conexión con la API.");
                }
            }

            await CargarProvinciasAsync();
            return View(cliente);
        }

        // FORMULARIO EDITAR
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                    return NotFound();

                var content = await response.Content.ReadAsStringAsync();
                var cliente = JsonSerializer.Deserialize<ClienteUpdateDTO>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                await CargarProvinciasAsync();
                return View(cliente);
            }
            catch
            {
                ViewBag.Error = "Error al cargar los datos del cliente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ClienteUpdateDTO cliente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{id}", cliente);
                    if (response.IsSuccessStatusCode)
                        return RedirectToAction(nameof(Index));

                    ModelState.AddModelError("", "Error al actualizar el cliente.");
                }
                catch
                {
                    ModelState.AddModelError("", "Error de conexión con la API.");
                }
            }

            await CargarProvinciasAsync();
            return View(cliente);
        }

        // ELIMINAR
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                return NotFound();
            }
            catch
            {
                return StatusCode(500, "Error al eliminar el cliente.");
            }
        }

        // CIUDADES dinámicas según provincia
        [HttpGet]
        public async Task<IActionResult> GetCiudades(string provinciaNombre)
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(
                    $"https://apis.datos.gob.ar/georef/api/localidades?provincia={Uri.EscapeDataString(provinciaNombre)}&max=5000");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<CiudadesResponse>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return Json(data.Localidades);
                }
            }
            catch { }

            return Json(new List<CiudadDTO>());
        }

        // MÉTODO AUXILIAR para provincias
        private async Task CargarProvinciasAsync()
        {
            using var http = new HttpClient();
            var provinciasResponse = await http.GetAsync("https://apis.datos.gob.ar/georef/api/provincias?campos=id,nombre");
            if (provinciasResponse.IsSuccessStatusCode)
            {
                var provinciasContent = await provinciasResponse.Content.ReadAsStringAsync();

                // DTO auxiliar
                var provinciasWrapper = JsonSerializer.Deserialize<ProvinciaWrapper>(provinciasContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                ViewBag.Provincias = provinciasWrapper?.Provincias
                    .OrderBy(p => p.Nombre)
                    .ToList() ?? new List<ProvinciaDTO>();
            }
            else
            {
                ViewBag.Provincias = new List<ProvinciaDTO>();
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreatePartial()
        {
            await CargarProvinciasAsync();
            return PartialView("~/Views/Shared/Modals/_ModalCreateCliente.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreatePartial([FromBody] ClienteCreateDTO cliente)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            try
            {
                var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, cliente);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var nuevoCliente = JsonSerializer.Deserialize<ClienteReadDTO>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // Devolvés el cliente creado como JSON
                    return Json(new { success = true, cliente = nuevoCliente });
                }

                return BadRequest("Error al crear el cliente");
            }
            catch
            {
                return StatusCode(500, "Error de conexión con la API");
            }
        }

    }
}

