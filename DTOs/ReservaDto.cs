namespace SistemaDeOptimizacionAPI.DTOs
{
    public class ReservaDto
    {
        public int ReservaID { get; set; }
        public int PerroID { get; set; }
        public string UsuarioID { get; set; }
        public int ServicioID { get; set; }
        public DateTime FechaReserva { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; }
        public int? PromocionID { get; set; }
        public decimal PrecioTotal { get; set; }
    }
}
