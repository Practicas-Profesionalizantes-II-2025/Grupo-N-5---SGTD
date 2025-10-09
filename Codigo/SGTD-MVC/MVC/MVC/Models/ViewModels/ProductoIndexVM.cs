namespace MVC.Models.ViewModels
{
    public class ProductoIndexVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; }
        public int DisciplinaId { get; set; }
        public string DisciplinaNombre { get; set; }
        public List<int> ProveedorIds { get; set; } = new List<int>();
        public List<string> ProveedorNombres { get; set; } = new List<string>();
    }
}
