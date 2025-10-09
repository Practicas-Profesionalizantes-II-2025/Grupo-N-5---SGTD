using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.FacturaDTOs
{
    public class FacturaProductoCreateDTO
    {
        [Required]
        public int ProductoId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal PrecioUnitario { get; set; }
    }
}
