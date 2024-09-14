namespace SistemaDeOptimizacionAPI.Models
{
    public class Servicio
    {
        public int ServicioID { get; set; }
        public string NombreServicio { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }

        public ICollection<Reserva> Reservas { get; set; }
    }
}
