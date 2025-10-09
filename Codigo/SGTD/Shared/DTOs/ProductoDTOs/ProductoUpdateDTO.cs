using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.ProductoDTOs
{
    public class ProductoUpdateDTO
    {
        [Required(ErrorMessage = "El ID es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID debe ser mayor a 0.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser igual o mayor a 0.")]
        public int Cantidad { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser igual o mayor a 0.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estado válido.")]
        public int EstadoId { get; set; }

        [Required(ErrorMessage = "La disciplina es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una disciplina válida.")]
        public int DisciplinaId { get; set; }
        public List<int> ProveedorIds { get; set; }
    }
}
