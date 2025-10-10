using Riok.Mapperly.Abstractions;
using Shared.DTOs.FacturaDTOs;
using Shared.Entidades;
using System.Linq;

namespace Service.Mappers
{
    [Mapper]
    public partial class FacturaMapper
    {
        public partial Factura ToEntity(FacturaCreateDTO dto);
        public partial FacturaReadDTO ToReadDto(Factura factura);
        public List<FacturaReadDTO> ToReadDtoList(IEnumerable<Factura> entities)
            => entities.Select(ToReadDto).ToList();

        // --- Agregar productos a la entidad Factura desde el DTO de creación ---
        public void CreateMapProductos(FacturaCreateDTO dto, Factura entity)
        {
            if (dto?.Productos == null) return;

            foreach (var p in dto.Productos)
            {
                entity.FacturaProductos.Add(new FacturaProducto
                {
                    ProductoId = p.ProductoId,
                    Cantidad = p.Cantidad,
                    PrecioUnitario = p.PrecioUnitario
                });
            }
        }

        public FacturaReadDTO ToReadDtoWithProductos(Factura factura)
        {
            var dto = ToReadDto(factura); // mapping básico generado por Mapperly

            // Mapear la colección intermedia manualmente
            dto.Productos = factura.FacturaProductos?
                .Select(fp => new FacturaProductoReadDTO
                {
                    ProductoId = fp.ProductoId,
                    Cantidad = fp.Cantidad,
                    PrecioUnitario = fp.PrecioUnitario,
                    Nombre = fp.Producto?.Nombre // si tu Producto tiene Nombre y el DTO también
                })
                .ToList() ?? new List<FacturaProductoReadDTO>();

            return dto;
        }

        public List<FacturaReadDTO> ToReadDtoListWithProductos(IEnumerable<Factura> entities)
            => entities.Select(ToReadDtoWithProductos).ToList();
    }
}
