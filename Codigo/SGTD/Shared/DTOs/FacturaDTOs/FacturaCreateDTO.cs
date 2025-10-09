using Shared.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.FacturaDTOs
{
    public class FacturaCreateDTO
    {
        [Required]
        public DateTime FechaEmision { get; set; }

        [Required, StringLength(150)]
        public string DireccionFiscal { get; set; }

        [Required]
        public int IdFiscal { get; set; }

        [Required, StringLength(100)]
        public string RazonSocial { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public List<FacturaProductoCreateDTO> Productos { get; set; } = new();
    }
}
