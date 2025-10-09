using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.ProductoDTOs
{
    public class ProductoReadDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public int EstadoId { get; set; }
        public int DisciplinaId { get; set; }
        public List<int> ProveedorIds { get; set; }
    }
}
