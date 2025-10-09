using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entidades
{
    public class Producto : EntidadBase
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }

        public Estado Estado { get; set; }
        public int EstadoId { get; set; }

        public Disciplina Disciplina { get; set; }
        public int DisciplinaId { get; set; }

        public ICollection<ProductoProveedor> ProductoProveedor { get; set; } = new List<ProductoProveedor>();
    }
}
