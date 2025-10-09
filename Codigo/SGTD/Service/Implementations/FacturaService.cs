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
        private readonly FacturaMapper _mapper = new FacturaMapper();

        public FacturaService(IFacturaRepository facturaRepository)
        {
            _facturaRepository = facturaRepository;
        }

        public async Task<List<FacturaReadDTO>> ObtenerTodosAsync()
        {
           // var factura = await _facturaRepository.FindAllAsync();

            var facturasConRelaciones = await _facturaRepository.Query()
                  .Include(f => f.FacturaProductos)
                      .ThenInclude(fp => fp.Producto)
                  .Include(f => f.Cliente)
                  .Include(f => f.Usuario)
                  .ToListAsync();

            return _mapper.ToReadDtoList(facturasConRelaciones);
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

            return _mapper.ToReadDto(factura);
        }

        public async Task<FacturaReadDTO> CrearAsync(FacturaCreateDTO dto)
        {
            var factura = new Factura
            {
                FechaEmision = dto.FechaEmision,
                DireccionFiscal = dto.DireccionFiscal,
                IdFiscal = dto.IdFiscal,
                RazonSocial = dto.RazonSocial,
                UsuarioId = dto.UsuarioId,
                ClienteId = dto.ClienteId,
                FacturaProductos = dto.Productos.Select(p => new FacturaProducto
                {
                    ProductoId = p.ProductoId,
                    Cantidad = p.Cantidad,
                    PrecioUnitario = p.PrecioUnitario
                }).ToList()
            };

            // Calcular monto total
            factura.Monto = factura.FacturaProductos.Sum(p => p.Cantidad * (int)p.PrecioUnitario);

            await _facturaRepository.Create(factura);

            var facturaConRelaciones = await _facturaRepository.Query()
                .Include(f => f.FacturaProductos)
                    .ThenInclude(fp => fp.Producto)
                        .Include(f => f.Cliente)
                             .Include(f => f.Usuario)
                                    .FirstOrDefaultAsync(f => f.Id == factura.Id);

            return _mapper.ToReadDto(factura);
        }

        public async Task Eliminar(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.");

            var estado = await _facturaRepository.ObtenerPorId(id);
            if (estado == null)
                throw new KeyNotFoundException($"No se encontró ninguna factura con ID {id}.");

            await _facturaRepository.Delete(estado);
        }
    }
}
