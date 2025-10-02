using Microsoft.AspNetCore.Mvc;
using MVC.Models.DTOs.ProveedorDto;

namespace MVC.Controllers
{
    using global::MVC.Models.DTOs.RubroDto;
    using global::MVC.Models.Entity;
    using Microsoft.AspNetCore.Authorization;
    using System.Text.Json;

    namespace MVC.Controllers
    {
        [Authorize(Roles = "Admin")]
        public class ProveedoresController : Controller
        {
            private readonly HttpClient _httpClient;
            private readonly string _apiBaseUrl = "proveedor"; // endpoint base de tu API
            private readonly string _apiRubroUrl = "rubro";

            public ProveedoresController(IHttpClientFactory httpClientFactory)
            {
                _httpClient = httpClientFactory.CreateClient("ProveedoresApi");
            }

            public async Task<IActionResult> Index()
            {
                try
                {
                    var response = await _httpClient.GetAsync(_apiBaseUrl);
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return View(new List<ProveedorReadDTO>());

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var proveedores = JsonSerializer.Deserialize<List<ProveedorReadDTO>>(content,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        return View(proveedores);
                    }
                }
                catch (Exception ex)
                {
                    // Log error si es necesario
                    ViewBag.Error = "Error al cargar los proveedores";
                }

                return View(new List<ProveedorReadDTO>());
            }

            [HttpGet]
            public async Task<IActionResult> Create()
            {
                try
                {
                    var response = await _httpClient.GetAsync(_apiRubroUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var rubros = JsonSerializer.Deserialize<List<RubroReadDTO>>(content,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        ViewBag.Rubros = rubros;
                    }
                    else
                    {
                        ViewBag.Rubros = new List<RubroReadDTO>();
                    }
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
                catch (Exception ex)
                {
                    ViewBag.Rubros = new List<RubroReadDTO>();
                }

                return View();
            }

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

            [HttpPost]
            public async Task<IActionResult> Create(ProveedorCreateDTO proveedor)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, proveedor);

                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction(nameof(Index));
                        }

                        ModelState.AddModelError("", "Error al crear el proveedor");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error de conexión con la API");
                    }
                }
                return View(proveedor);
            }

            [HttpGet]   
            public async Task<IActionResult> Edit(int id)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var proveedor = JsonSerializer.Deserialize<ProveedorUpdateDTO>(content,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        // Cargar Rubros
                        var rubroResponse = await _httpClient.GetAsync(_apiRubroUrl);
                        if (rubroResponse.IsSuccessStatusCode)
                        {
                            var rubroContent = await rubroResponse.Content.ReadAsStringAsync();
                            var rubros = JsonSerializer.Deserialize<List<RubroReadDTO>>(rubroContent,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            ViewBag.Rubros = rubros;
                        }

                        // Cargar Provincias
                        using var http = new HttpClient();
                        var provinciasResponse = await http.GetAsync("https://apis.datos.gob.ar/georef/api/provincias?campos=id,nombre");
                        if (provinciasResponse.IsSuccessStatusCode)
                        {
                            var provinciasContent = await provinciasResponse.Content.ReadAsStringAsync();
                            var provinciasWrapper = JsonSerializer.Deserialize<ProvinciaWrapper>(provinciasContent,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            ViewBag.Provincias = provinciasWrapper?.Provincias.OrderBy(p => p.Nombre).ToList();
                        }

                        return View(proveedor);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al cargar el proveedor";
                }

                return NotFound();
            }

            [HttpPost]
            public async Task<IActionResult> Edit(int id, ProveedorUpdateDTO proveedor)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{id}", proveedor);

                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction(nameof(Index));
                        }

                        ModelState.AddModelError("", "Error al actualizar el proveedor");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error de conexión con la API");
                    }
                }

                // recargar combos si algo falla
                var rubroResponse = await _httpClient.GetAsync(_apiRubroUrl);
                if (rubroResponse.IsSuccessStatusCode)
                {
                    var rubroContent = await rubroResponse.Content.ReadAsStringAsync();
                    var rubros = JsonSerializer.Deserialize<List<RubroReadDTO>>(rubroContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    ViewBag.Rubros = rubros;
                }

                using var http = new HttpClient();
                var provinciasResponse = await http.GetAsync("https://apis.datos.gob.ar/georef/api/provincias?campos=id,nombre");
                if (provinciasResponse.IsSuccessStatusCode)
                {
                    var provinciasContent = await provinciasResponse.Content.ReadAsStringAsync();
                    var provinciasWrapper = JsonSerializer.Deserialize<ProvinciaWrapper>(provinciasContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    ViewBag.Provincias = provinciasWrapper?.Provincias.OrderBy(p => p.Nombre).ToList();
                }

                return View(proveedor);
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
                    return StatusCode(500, "Error al eliminar el proveedor");
                }
            }
        }
    }
    public class ProvinciaWrapper
    {
        public List<ProvinciaDTO> Provincias { get; set; }
    }

    public class ProvinciaDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
    }

    public class CiudadesResponse
    {
        public List<CiudadDTO> Localidades { get; set; }
    }

    public class CiudadDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
    }
}
