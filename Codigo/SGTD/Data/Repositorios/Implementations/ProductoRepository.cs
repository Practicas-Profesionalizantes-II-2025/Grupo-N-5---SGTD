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

namespace Data.Implementations
{
    public class ProductoRepository : Repository<Producto>, IProductoRepository
    {
        public ProductoRepository(AppDbContext context) : base(context) { }
        public async Task<List<Producto>> FindByDisciplinaAsync(int disciplinaId)
        {
            return await _context.Productos
                .Where(p => p.DisciplinaId == disciplinaId)
                .ToListAsync();
        }
        public async Task<bool> ExistePorNombreAsync(string nombre, int? excludeUserId = null)
        {
            return await _context.Productos
                .AnyAsync(u => u.Nombre == nombre && (!excludeUserId.HasValue || u.Id != excludeUserId));
        }

        public async Task<List<Producto>> FindAllAsyncConProveedores()
        {
            return await _context.Productos
                .Include(p => p.ProductoProveedor)
                    .ThenInclude(rp => rp.Proveedor) // si también querés los datos del Rubro
                .ToListAsync();
        }

        public async Task<Producto> ObtenerPorIdConProveedores(int id)
        {
            return await _context.Productos
                .Include(p => p.ProductoProveedor)
                    .ThenInclude(rp => rp.Proveedor)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Producto>> FindActivosAsync()
        {
            return await _context.Productos
                .Where(p => p.EstadoId == 1) // 1 = Activo
                .ToListAsync();
        }
    }
}
