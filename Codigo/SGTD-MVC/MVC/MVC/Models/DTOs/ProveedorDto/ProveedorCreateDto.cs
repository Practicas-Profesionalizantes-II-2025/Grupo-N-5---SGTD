using System.ComponentModel.DataAnnotations;

namespace MVC.Models.DTOs.ProveedorDto
{
    public class ProveedorCreateDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El CUIT es obligatorio.")]
        [StringLength(13, ErrorMessage = "El CUIT no puede superar los 13 caracteres.")] // formato típico XX-XXXXXXXX-X
        public string Cuit { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(150, ErrorMessage = "La dirección no puede superar los 150 caracteres.")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [StringLength(50, ErrorMessage = "La ciudad no puede superar los 50 caracteres.")]
        public string Ciudad { get; set; }  

        [Required(ErrorMessage = "La provincia es obligatoria.")]
        [StringLength(50, ErrorMessage = "La provincia no puede superar los 50 caracteres.")]
        public string Provincia { get; set; }

        [Required(ErrorMessage = "El rubro es obligatorio.")]
        public List<int> RubroIds { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public int EstadoId { get; set; }
    }
}
