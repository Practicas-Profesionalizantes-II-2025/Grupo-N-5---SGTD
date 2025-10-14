using Data.Context;
using Data.Contracts;
using Data.Repositorios.Implementations;
using Microsoft.EntityFrameworkCore;
using Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data.Implementations
{
    public class ProveedorRepository : Repository<Proveedor>, IProveedorRepository
    {
        public ProveedorRepository(AppDbContext context) : base(context) { }
        
        public async Task<bool> ExistePorCuitAsync(string cuit, int? excludeUserId = null)
        {
            return await _context.Proveedores
                .AnyAsync(u => u.Cuit == cuit && (excludeUserId == null || u.Id != excludeUserId));
        }

        public async Task<List<Proveedor>> FindAllAsyncConRubros()
        {
            return await _context.Proveedores
                .Include(p => p.RubrosProveedor)
                    .ThenInclude(rp => rp.Rubro) // si también querés los datos del Rubro
                .ToListAsync();
        }

        public async Task<Proveedor> ObtenerPorIdConRubros(int id)
        {
            return await _context.Proveedores
                .Include(p => p.RubrosProveedor)
                    .ThenInclude(rp => rp.Rubro)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Proveedor>> FindActivosAsync()
        {
            return await _context.Proveedores
                .Where(p => p.EstadoId == 1) // 1 = Activo
                .ToListAsync();
        }
    }
}
