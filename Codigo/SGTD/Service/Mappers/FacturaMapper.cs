using Riok.Mapperly.Abstractions;
using Shared.DTOs.ClienteDTOs;
using Shared.DTOs.FacturaDTOs;
using Shared.DTOs.RolDTOs;
using Shared.DTOs.RubroDTOs;
using Shared.Entidades;


namespace Service.Mappers
{
    [Mapper]
    public partial class FacturaMapper
    {
        public partial Factura ToEntity(FacturaCreateDTO dto);
        public partial FacturaReadDTO ToReadDto(Factura factura);
        public List<FacturaReadDTO> ToReadDtoList(IEnumerable<Factura> entities)
        => entities.Select(ToReadDto).ToList();
    }
}
