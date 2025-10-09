using Shared.DTOs.FacturaDTOs;
using System.ComponentModel.DataAnnotations;

namespace MVC.Models.DTOs.FacturaDto
{
    public class FacturaCreateDTO
    {
        [Required(ErrorMessage = "La fecha de emisión es obligatoria.")]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La dirección fiscal es obligatoria.")]
        [StringLength(150, ErrorMessage = "La dirección fiscal no puede superar los 150 caracteres.")]
        public string DireccionFiscal { get; set; }

        [Required(ErrorMessage = "El ID fiscal es obligatorio.")]
        public int IdFiscal { get; set; }

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        [StringLength(100, ErrorMessage = "La razón social no puede superar los 100 caracteres.")]
        public string RazonSocial { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Debe agregar al menos un producto.")]
        public List<FacturaProductoCreateDTO> Productos { get; set; } = new();

        // Campo auxiliar que viene desde el input hidden del form
        public string ProductosJson { get; set; }
    }
}
