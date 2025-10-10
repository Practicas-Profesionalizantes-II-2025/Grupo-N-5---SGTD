using Shared.DTOs.FacturaDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IFacturaService
    {
        Task<List<FacturaReadDTO>> ObtenerTodosAsync();
        Task<FacturaReadDTO> ObtenerPorIdAsync(int id);
        Task<FacturaReadDTO> CrearAsync(FacturaCreateDTO dto);
        Task Eliminar(int id);
    }
}
