using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Service.Implementations;
using Shared.DTOs.ProductoDTOs;
using Shared.Entidades;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearProducto([FromBody] ProductoCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Los datos del cliente no pueden ser nulos.");
            try
            {
                var producto = await _productoService.CrearAsync(dto);
                if (producto != null)
                    return Ok(producto);

                return Conflict("Ya existe un producto con esos datos.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error interno al crear el producto.");
            }
        }

        [HttpGet("filtrarPorDisciplina/{disciplinaId}")]
        public async Task<IActionResult> FiltrarPorDisciplina(int disciplinaId)
        {
            var productos = await _productoService.ObtenerPorDisciplinaAsync(disciplinaId);
            return Ok(productos);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProducto()
        {
            try
            {
                var producto = await _productoService.ObtenerTodosAsync();
                if (producto == null || !producto.Any())
                    return NoContent();

                return Ok(producto);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al obtener los productos.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerProductoPorId(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a cero.");
            try
            {
                var producto = await _productoService.ObtenerPorIdAsync(id);
                if (producto != null)

                    return Ok(producto);
                return BadRequest(producto);

            }
            catch (Exception)
            {
                return NotFound($"No se encontró un producto con ID {id}.");
            }
        
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] ProductoUpdateDTO dto)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a cero.");
            if (dto == null)
                return BadRequest("Los datos de actualización no pueden ser nulos.");

            try
            {
                var producto = await _productoService.Editar(id, dto);
                if (producto != null)
                    return Ok(producto);


                return BadRequest(producto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno al actualizar el producto.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            try
            {
                await _productoService.Eliminar(id);
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
                return StatusCode(500, "Ocurrio un error interno al eliminar el producto");
            }

        }
    }
}
