using Shared.DTOs.FacturaDTOs;
using Shared.DTOs.ProductoDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IProductoService
    {
        Task<List<ProductoReadDTO>> ObtenerPorDisciplinaAsync(int disciplinaId);
        Task<List<ProductoReadDTO>> ObtenerTodosAsync();
        Task<ProductoReadDTO> ObtenerPorIdAsync(int id);
        Task<ProductoReadDTO> CrearAsync(ProductoCreateDTO dto);
        Task RestarStockAsync(IEnumerable<FacturaProductoCreateDTO> items);
        Task<ProductoReadDTO> Editar(int id, ProductoUpdateDTO dto);
        Task Eliminar(int id);
    }
}
