using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entidades
{
    public class Factura : EntidadBase
    {
        public DateTime FechaEmision { get; set; }
        public decimal Monto { get; set; }
        public string DireccionFiscal { get; set; }
        public int IdFiscal { get; set; }
        public string RazonSocial { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        // Relación con productos
        public ICollection<FacturaProducto> FacturaProductos { get; set; } = new List<FacturaProducto>();
    }
}
