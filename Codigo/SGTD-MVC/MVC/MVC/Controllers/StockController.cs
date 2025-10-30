using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Models.DTOs.DisciplinaDto;
using MVC.Models.DTOs.EstadoDto;
using MVC.Models.DTOs.ProductoDto;
using MVC.Models.DTOs.ProveedorDto;
using MVC.Models.DTOs.RubroDto;
using MVC.Models.Entity;
using MVC.Models.ViewModels;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace MVC.Controllers
{
    [Authorize(Roles = "Admin,Empleado,Contador")]
    public class StockController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "producto";
        private readonly string _apiDisciplinaUrl = "disciplina";
        private readonly string _apiProveedorUrl = "proveedor/activos";
        private readonly string _apiEstadoUrl = "estado";


        public StockController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ProductosApi");
        }

        public async Task<IActionResult> Index()
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await _httpClient.GetAsync(_apiBaseUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return View(new List<ProductoIndexVM>());

                var productoDto = new List<ProductoReadDTO>();
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    productoDto = JsonSerializer.Deserialize<List<ProductoReadDTO>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                var estadoResponse = await _httpClient.GetAsync(_apiEstadoUrl);
                var estados = new List<EstadoReadDTO>();
                if (estadoResponse.IsSuccessStatusCode)
                {
                    var estadoContent = await estadoResponse.Content.ReadAsStringAsync();
                    estados = JsonSerializer.Deserialize<List<EstadoReadDTO>>(estadoContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                var disciplinasResponse = await _httpClient.GetAsync(_apiDisciplinaUrl);
                var disciplinas = new List<RubroReadDTO>();
                if (disciplinasResponse.IsSuccessStatusCode)
                {
                    var rubroContent = await disciplinasResponse.Content.ReadAsStringAsync();
                    disciplinas = JsonSerializer.Deserialize<List<RubroReadDTO>>(rubroContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                var proveedoresResponse = await _httpClient.GetAsync(_apiProveedorUrl);
                var proveedores = new List<ProveedorReadDTO>();
                if (proveedoresResponse.IsSuccessStatusCode)
                {
                    var proveedorContent = await proveedoresResponse.Content.ReadAsStringAsync();
                    proveedores = JsonSerializer.Deserialize<List<ProveedorReadDTO>>(proveedorContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                var productoVM = productoDto.Select(p => new ProductoIndexVM
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Cantidad = p.Cantidad,
                    Precio = p.Precio,
                    DisciplinaId = p.DisciplinaId,
                    DisciplinaNombre = disciplinas.FirstOrDefault(d => d.Id == p.DisciplinaId)?.Nombre ?? "Sin disciplina",
                    ProveedorIds = p.ProveedorIds,
                    ProveedorNombres = proveedores
                        .Where(pr => p.ProveedorIds.Contains(pr.Id))
                        .Select(pr => pr.Nombre)
                        .ToList(),
                    EstadoId = p.EstadoId,
                    EstadoNombre = estados.FirstOrDefault(e => e.Id == p.EstadoId)?.Nombre ?? "Sin estado",
                }).ToList();

                foreach (var producto in productoVM)
                {
                    MetricsCollector.ProductsInStock
                        .WithLabels(producto.DisciplinaNombre, producto.Id.ToString())
                        .Set(producto.Cantidad);
                }

                return View(productoVM);
            }
            catch (Exception)
            {
                ViewBag.Error = "Error al cargar los proveedores";
                return View(new List<ProveedorIndexVm>());
            }
            finally
            {
                stopwatch.Stop();
                MetricsCollector.HttpRequestDuration
                    .WithLabels("GET", "Stock/Index", Response?.StatusCode.ToString() ?? "500")
                    .Observe(stopwatch.Elapsed.TotalSeconds);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var disciplinaResponse = await _httpClient.GetAsync(_apiDisciplinaUrl);
                if (disciplinaResponse.IsSuccessStatusCode)
                {
                    var content = await disciplinaResponse.Content.ReadAsStringAsync();
                    var disciplinas = JsonSerializer.Deserialize<List<DisciplinaReadDTO>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    ViewBag.Disciplinas = disciplinas;
                }
                else
                {
                    ViewBag.Disciplinas = new List<DisciplinaReadDTO>();
                }

                var proveedorResponse = await _httpClient.GetAsync(_apiProveedorUrl);
                if (proveedorResponse.IsSuccessStatusCode)
                {
                    var content = await proveedorResponse.Content.ReadAsStringAsync();
                    var proveedores = JsonSerializer.Deserialize<List<ProveedorReadDTO>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    ViewBag.Proveedores = proveedores;
                }
                else
                {
                    ViewBag.Proveedores = new List<ProveedorReadDTO>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Disciplinas = new List<DisciplinaReadDTO>();
                ViewBag.Proveedores = new List<ProveedorReadDTO>();
            }

            return View();
        }

        //Obtener Disciplinas de forma dinámica
        [HttpGet]
        public async Task<IActionResult> GetDisciplinas()
        {
            var response = await _httpClient.GetAsync(_apiDisciplinaUrl);
            if (!response.IsSuccessStatusCode)
                return Json(new List<DisciplinaReadDTO>());

            var content = await response.Content.ReadAsStringAsync();
            var disciplinas = JsonSerializer.Deserialize<List<DisciplinaReadDTO>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return Json(disciplinas);
        }


        [HttpPost]
        public async Task<IActionResult> Create(ProductoCreateDTO producto)
        {
            if (ModelState.IsValid)
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, producto);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError("", "Error al crear el producto");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error de conexión con la API");
                }
                finally
                {
                    stopwatch.Stop();
                    MetricsCollector.HttpRequestDuration
                        .WithLabels("POST", "Stock/UpdateStock", Response?.StatusCode.ToString() ?? "500")
                        .Observe(stopwatch.Elapsed.TotalSeconds);
                }
            }
            return View(producto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Obtener el producto por id desde la API
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                    return NotFound();

                var content = await response.Content.ReadAsStringAsync();
                var producto = JsonSerializer.Deserialize<ProductoUpdateDTO>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (producto == null)
                    return NotFound();

                // Traer listas de disciplinas y proveedores
                var disciplinaResponse = await _httpClient.GetAsync(_apiDisciplinaUrl);
                ViewBag.Disciplinas = disciplinaResponse.IsSuccessStatusCode
                    ? JsonSerializer.Deserialize<List<DisciplinaReadDTO>>(await disciplinaResponse.Content.ReadAsStringAsync(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    : new List<DisciplinaReadDTO>();

                var proveedorResponse = await _httpClient.GetAsync(_apiProveedorUrl);
                ViewBag.Proveedores = proveedorResponse.IsSuccessStatusCode
                    ? JsonSerializer.Deserialize<List<ProveedorReadDTO>>(await proveedorResponse.Content.ReadAsStringAsync(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    : new List<ProveedorReadDTO>();

                // Mapear a UpdateDTO para enviar al POST
                var productoUpdate = new ProductoUpdateDTO
                {
                    Nombre = producto.Nombre,
                    Cantidad = producto.Cantidad,
                    Precio = producto.Precio,
                    EstadoId = producto.EstadoId,
                    DisciplinaId = producto.DisciplinaId,
                    ProveedorIds = producto.ProveedorIds
                };

                return View(productoUpdate);
            }
            catch
            {
                return StatusCode(500, "Error al cargar el producto");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductoUpdateDTO producto)
        {
            if (!ModelState.IsValid)
            {
                // Si hay error de validación, volver a traer listas para el form
                var disciplinaResponse = await _httpClient.GetAsync(_apiDisciplinaUrl);
                ViewBag.Disciplinas = disciplinaResponse.IsSuccessStatusCode
                    ? JsonSerializer.Deserialize<List<DisciplinaReadDTO>>(await disciplinaResponse.Content.ReadAsStringAsync(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    : new List<DisciplinaReadDTO>();

                var proveedorResponse = await _httpClient.GetAsync(_apiProveedorUrl);
                ViewBag.Proveedores = proveedorResponse.IsSuccessStatusCode
                    ? JsonSerializer.Deserialize<List<ProveedorReadDTO>>(await proveedorResponse.Content.ReadAsStringAsync(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    : new List<ProveedorReadDTO>();

                return View(producto);
            }
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{id}", producto);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Error al actualizar el producto: {errorContent}");
            }
            catch
            {
                ModelState.AddModelError("", "Error de conexión con la API");
            }
            finally
            {
                stopwatch.Stop();
                MetricsCollector.HttpRequestDuration
                    .WithLabels("POST", "Stock/UpdateStock", Response?.StatusCode.ToString() ?? "500")
                    .Observe(stopwatch.Elapsed.TotalSeconds);
            }

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(ProductoStockVM vm)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{vm.Id}");
                if (!response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

                var body = await response.Content.ReadAsStringAsync();
                var productoDto = JsonSerializer.Deserialize<ProductoUpdateDTO>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (productoDto == null) return RedirectToAction(nameof(Index));

                productoDto.Cantidad += vm.Cantidad;
                if (productoDto.Cantidad < 0) productoDto.Cantidad = 0;

                var json = JsonSerializer.Serialize(productoDto);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var putResponse = await _httpClient.PutAsync($"{_apiBaseUrl}/{productoDto.Id}", httpContent);

                if (!putResponse.IsSuccessStatusCode)
                    TempData["Error"] = "No se pudo actualizar el stock";

                MetricsCollector.ProductsInStock
                    .WithLabels("Desconocido", productoDto.Id.ToString())
                    .Set(productoDto.Cantidad);

                return RedirectToAction(nameof(Index));
            }
            finally
            {
                stopwatch.Stop();
                MetricsCollector.HttpRequestDuration
                    .WithLabels("POST", "Stock/UpdateStock", Response?.StatusCode.ToString() ?? "500")
                    .Observe(stopwatch.Elapsed.TotalSeconds);
            }
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
