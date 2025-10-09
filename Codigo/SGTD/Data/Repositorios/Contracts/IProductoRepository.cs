using Data.Repositorios.Contracts;
using Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IProductoRepository : IRepository<Producto>
    {
        Task<List<Producto>> FindByDisciplinaAsync(int disciplinaId);
        Task<List<Producto>> FindAllAsyncConProveedores();
        Task<Producto> ObtenerPorIdConProveedores(int id);
        Task<bool> ExistePorNombreAsync(string nombre, int? excludeUserId = null);
    }
}
