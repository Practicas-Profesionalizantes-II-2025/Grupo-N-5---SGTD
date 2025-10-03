using Microsoft.AspNetCore.Mvc;
using MVC.Models.DTOs.DisciplinaDto;

namespace MVC.Controllers
{
    public class DisciplinasController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "disciplina";

        public DisciplinasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DisciplinasApi");
        }

        // Trae los rubros y devuelve la vista parcial (para el modal)
        public async Task<IActionResult> GetDisciplinas()
        {
            var response = await _httpClient.GetAsync(_apiBaseUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return PartialView("Modals/_TablaDisciplinas", new List<DisciplinaReadDTO>());

            response.EnsureSuccessStatusCode();

            var disciplinas = await response.Content.ReadFromJsonAsync<List<DisciplinaReadDTO>>();
            return PartialView("Modals/_TablaDisciplinas", disciplinas ?? new List<DisciplinaReadDTO>());
        }

        [HttpPost]
        public async Task<IActionResult> CrearDisciplina(DisciplinaCreateDTO disciplina)
        {
            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            var postResp = await _httpClient.PostAsJsonAsync(_apiBaseUrl, disciplina);

            if (!postResp.IsSuccessStatusCode)
                return BadRequest("Error al crear disciplina");

            // Traigo la lista actualizada
            var getResp = await _httpClient.GetAsync(_apiBaseUrl);
            if (getResp.StatusCode == System.Net.HttpStatusCode.NoContent)
                return PartialView("Modals/_TablaRubros", new List<DisciplinaReadDTO>());

            getResp.EnsureSuccessStatusCode();
            var disciplinas = await getResp.Content.ReadFromJsonAsync<List<DisciplinaReadDTO>>() ?? new List<DisciplinaReadDTO>();

            return PartialView("Modals/_TablaDisciplinas", disciplinas);
        }

        [HttpPost]
        public async Task<IActionResult> ModificarDisciplina(DisciplinaUpdateDTO disciplina)
        {
            if (!ModelState.IsValid) return BadRequest("Datos inválidos");

            var putResp = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{disciplina.Id}", disciplina);
            if (!putResp.IsSuccessStatusCode) return BadRequest("Error al modificar disciplina");

            // Devolver tabla actualizada
            var getResp = await _httpClient.GetAsync(_apiBaseUrl);
            var disciplinas = await getResp.Content.ReadFromJsonAsync<List<DisciplinaReadDTO>>() ?? new List<DisciplinaReadDTO>();
            return PartialView("Modals/_TablaDisciplinas", disciplinas);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarDisciplina(int id)
        {
            var delResp = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");
            if (!delResp.IsSuccessStatusCode) return BadRequest("Error al eliminar disciplina");

            var getResp = await _httpClient.GetAsync(_apiBaseUrl);
            var disciplina = await getResp.Content.ReadFromJsonAsync<List<DisciplinaReadDTO>>() ?? new List<DisciplinaReadDTO>();
            return PartialView("Modals/_TablaDisciplinas", disciplina);
        }
    }
}
