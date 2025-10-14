using MVC.Models.DTOs.FacturaDTO;

namespace MVC.Models.ViewModels
{
    public class ReportesIndexVM
    {
        public int Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public string DireccionFiscal { get; set; }
        public int IdFiscal { get; set; }
        public string RazonSocial { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public List<FacturaProductoReadDTO> Productos { get; set; } = new();
        public decimal Total { get; set; }
    }
}
