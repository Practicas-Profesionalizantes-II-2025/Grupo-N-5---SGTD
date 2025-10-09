using Data.Contracts;
using Service.Contracts;
using Service.Mappers;
using Shared.DTOs.ProductoDTOs;
using Shared.Entidades;

namespace Service.Implementations
{
    public class ProductoService : IProductoService
    {
        private IProductoRepository _ProductoRepository;
        private readonly ProductoMapper _mapper = new ProductoMapper();

        public ProductoService(IProductoRepository productoRepository)
        {
            _ProductoRepository = productoRepository;
        }
        public async Task<List<ProductoReadDTO>> ObtenerPorDisciplinaAsync(int disciplinaId)
        {
            if (disciplinaId <= 0)
                throw new ArgumentException("El ID de disciplina debe ser mayor a cero.");

            // Asumiendo que tu repositorio tiene un método que permite filtrar por disciplina
            var productos = await _ProductoRepository.FindByDisciplinaAsync(disciplinaId);

            return _mapper.ToReadDtoList(productos);
        }

        public async Task<List<ProductoReadDTO>> ObtenerTodosAsync()
        {
            var productos = await _ProductoRepository.FindAllAsyncConProveedores();
            return _mapper.ToReadDtoList(productos);
        }

        public async Task<ProductoReadDTO> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            var producto = await _ProductoRepository.ObtenerPorIdConProveedores(id);
            if (producto == null)
                throw new KeyNotFoundException($"No se encontró ningún producto con ID {id}.");

            return _mapper.ToReadDtoWithProveedores(producto);
        }

        public async Task<ProductoReadDTO> CrearAsync(ProductoCreateDTO dto)
        {
            await ValidarProductoCreateDTO(dto);

            var producto = _mapper.ToEntity(dto);
            _mapper.CreateMapProveedores(dto, producto);
            await _ProductoRepository.Create(producto);

            return _mapper.ToReadDtoWithProveedores(producto);
        }

        public async Task<ProductoReadDTO> Editar(int id, ProductoUpdateDTO dto)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            await ValidarProductoUpdateDTO(id, dto);

            var producto = await _ProductoRepository.ObtenerPorIdConProveedores(id);
            if (producto == null)
                throw new KeyNotFoundException($"No se encontró ningún producto con ID {id}.");

            producto.UpdatedDate = DateTime.Now;
            _mapper.UpdateEntity(dto, producto);
            _mapper.UpdateMapProveedores(dto, producto);
            await _ProductoRepository.Update(producto);

            return _mapper.ToReadDtoWithProveedores(producto);
        }

        public async Task Eliminar(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            var producto = await _ProductoRepository.ObtenerPorIdConProveedores(id);
            if (producto == null)
                throw new KeyNotFoundException($"No se encontro ningún producto con ID {id}.");

            await _ProductoRepository.Delete(producto); 
        }


        async private Task ValidarProductoCreateDTO(ProductoCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre del producto es obligatorio.");

            if (dto.Cantidad < 0)
                throw new ArgumentException("La cantidad no puede ser negativa.");

            if (dto.Precio < 0)
                throw new ArgumentException("El precio no puede ser negativo.");

            if (dto.EstadoId <= 0)
                throw new ArgumentException("El EstadoId debe ser válido.");

            if (dto.DisciplinaId <= 0)
                throw new ArgumentException("El DisciplinaId debe ser válido.");
            var hayProducto = _ProductoRepository.FindAll();
            if (hayProducto.Count() != 0)
            {
                if (await _ProductoRepository.ExistePorNombreAsync(dto.Nombre))
                    throw new ArgumentException("Ya existe un producto con ese nombre.", nameof(dto.Nombre));
            }
        }
        private async Task ValidarProductoUpdateDTO(int id, ProductoUpdateDTO dto)
        {
            if (await _ProductoRepository.ExistePorNombreAsync(dto.Nombre, id))
                throw new ArgumentException("Ya existe un producto con ese nombre.", nameof(dto.Nombre));
            
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre del producto es obligatorio.");

            if (dto.Cantidad < 0)
                throw new ArgumentException("La cantidad no puede ser negativa.");

            if (dto.Precio < 0)
                throw new ArgumentException("El precio no puede ser negativo.");

            if (dto.EstadoId <= 0)
                throw new ArgumentException("El EstadoId debe ser válido.");

            if (dto.DisciplinaId <= 0)
                throw new ArgumentException("El DisciplinaId debe ser válido.");
        }
    }
}

