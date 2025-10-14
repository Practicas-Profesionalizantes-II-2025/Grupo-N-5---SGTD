using Data.Repositorios.Contracts;
using Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IProveedorRepository : IRepository<Proveedor>
    {
        Task<List<Proveedor>> FindAllAsyncConRubros();
        Task<Proveedor> ObtenerPorIdConRubros(int id);
        Task<bool> ExistePorCuitAsync(string cuit, int? excludeUserId = null);

        Task<List<Proveedor>> FindActivosAsync();
    }
}
