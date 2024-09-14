namespace SistemaDeOptimizacionAPI.Models
{
    public class Reserva
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

        public Perro Perro { get; set; }
        public ApplicationUser Usuario { get; set; }
        public Servicio Servicio { get; set; }
        public Promocion Promocion { get; set; }
        public ICollection<ControlActividad> ControlActividades { get; set; }
    }
}
