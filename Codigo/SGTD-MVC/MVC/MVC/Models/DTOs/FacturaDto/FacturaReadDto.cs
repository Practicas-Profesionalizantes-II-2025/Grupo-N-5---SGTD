namespace MVC.Models.DTOs.FacturaDto
{
    public class FacturaReadDTO
    {
        public int Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public int Monto { get; set; }
        public string DireccionFiscal { get; set; }
        public int IdFiscal { get; set; }
        public string RazonSocial { get; set; }
        public int CantidadProductos { get; set; }
        public int ProductoId { get; set; }
        public int UsuarioId { get; set; }
        public int ClienteId { get; set; }
    }
}
