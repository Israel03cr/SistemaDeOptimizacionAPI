namespace SistemaDeOptimizacionAPI.DTOs
{
    public class PromocionDto
    {
        public int PromocionID { get; set; }
        public string CodigoPromocional { get; set; }
        public string Descripcion { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
    }
}
