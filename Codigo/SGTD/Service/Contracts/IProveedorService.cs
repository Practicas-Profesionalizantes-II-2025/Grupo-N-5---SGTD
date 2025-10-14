using Shared.DTOs.ProductoDTOs;
using Shared.DTOs.ProveedorDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IProveedorService
    {
        Task<List<ProveedorReadDTO>> ObtenerTodosAsync();
        Task<ProveedorReadDTO> ObtenerPorIdAsync(int id);
        Task<ProveedorReadDTO> CrearAsync(ProveedorCreateDTO dto);
        Task<ProveedorReadDTO> Editar(int id, ProveedorUpdateDTO dto);
        Task Eliminar(int id);

        Task<IEnumerable<ProveedorReadDTO>> ObtenerProveedoresActivosAsync();
    }
}
