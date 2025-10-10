namespace MVC.Models.DTOs.ProductoDto
{
    public class ProductoReadDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public int EstadoId { get; set; }
        public int DisciplinaId { get; set; }
        public List<int> ProveedorIds { get; set; } = new List<int>();

    }
}
