using System.ComponentModel.DataAnnotations;

namespace MVC.Models.DTOs.ProductoDto
{
    public class ProductoCreateDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser igual o mayor a 0.")]
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estado válido.")]
        public int EstadoId { get; set; }

        [Required(ErrorMessage = "La disciplina es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una disciplina válida.")]
        public int DisciplinaId { get; set; }

        public List<int> ProveedorIds { get; set; } = new List<int>();
    }
}
