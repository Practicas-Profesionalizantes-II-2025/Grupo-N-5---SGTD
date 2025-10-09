namespace Shared.DTOs.FacturaDTOs
{
    //public class FacturaReadDTO
    //{
    //    public int Id { get; set; }
    //    public DateTime FechaEmision { get; set; }
    //    public int Monto { get; set; }
    //    public string DireccionFiscal { get; set; }
    //    public int IdFiscal { get; set; }
    //    public string Descripcion { get; set; }
    //    public string RazonSocial { get; set; }
    //    public int CantidadProductos { get; set; }
    //    public int ProductoId { get; set; }
    //    public int UsuarioId { get; set; }
    //    public int ClienteId { get; set; }
    //}

    public class FacturaReadDTO
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

        // Se calcula sumando subtotales
        public decimal Total => Productos.Sum(p => p.Subtotal);
    }
}
