using Data.Contracts;
using Data.Repositorios.Contracts;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using Service.Mappers;
using Shared.DTOs.FacturaDTOs;
using Shared.Entidades;

namespace Service.Implementations
{
    public class FacturaService : IFacturaService
    {
        private IFacturaRepository _facturaRepository;
        private readonly IProductoService _productoService;
        private readonly FacturaMapper _mapper = new FacturaMapper();

        public FacturaService(IFacturaRepository facturaRepository, IProductoService productoService)
        {
            _facturaRepository = facturaRepository;
            _productoService = productoService;
        }

        public async Task<List<FacturaReadDTO>> ObtenerTodosAsync()
        {
            var facturasConRelaciones = await _facturaRepository.Query()
                  .Include(f => f.FacturaProductos)
                      .ThenInclude(fp => fp.Producto)
                  .Include(f => f.Cliente)
                  .Include(f => f.Usuario)
                  .ToListAsync();

            // Usar el mapping que incluye los productos
            return _mapper.ToReadDtoListWithProductos(facturasConRelaciones);
        }

        public async Task<FacturaReadDTO> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            var factura = await _facturaRepository.Query()
                .Include(f => f.FacturaProductos)
                    .ThenInclude(fp => fp.Producto)
                .Include(f => f.Cliente)
                .Include(f => f.Usuario)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (factura == null)
                throw new KeyNotFoundException($"No se encontró ninguna factura con ID {id}.");

            // Devolver DTO que incluye los productos
            return _mapper.ToReadDtoWithProductos(factura);
        }

        public async Task<FacturaReadDTO> CrearAsync(FacturaCreateDTO dto)
        {
            var factura = _mapper.ToEntity(dto);
            _mapper.CreateMapProductos(dto, factura);
            factura.Monto = factura.FacturaProductos.Sum(fp => fp.Cantidad * fp.PrecioUnitario);
            
            await _productoService.RestarStockAsync(dto.Productos);
            await _facturaRepository.Create(factura);          

            // Cargar la factura guardada con relaciones para devolver un DTO completo
            var facturaConRelaciones = await _facturaRepository.Query()
                .Include(f => f.FacturaProductos)
                    .ThenInclude(fp => fp.Producto)
                .Include(f => f.Cliente)
                .Include(f => f.Usuario)
                .FirstOrDefaultAsync(f => f.Id == factura.Id);

            return _mapper.ToReadDtoWithProductos(facturaConRelaciones ?? factura);
        }

        public async Task Eliminar(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            // Cargar la factura completa (incluye productos) antes de eliminar
            var facturaAEliminar = await _facturaRepository.Query()
                .Include(f => f.FacturaProductos)
                    .ThenInclude(fp => fp.Producto)
                .Include(f => f.Cliente)
                .Include(f => f.Usuario)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (facturaAEliminar == null)
                throw new KeyNotFoundException($"No se encontró ninguna factura con ID {id}.");

            await _facturaRepository.Delete(facturaAEliminar);
        }
    }
}
