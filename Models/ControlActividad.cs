namespace SistemaDeOptimizacionAPI.Models
{
    public class ControlActividad
    {
        public int ControlID { get; set; }
        public int ReservaID { get; set; }
        public int ActividadID { get; set; }
        public DateTime FechaActividad { get; set; }

        public Reserva Reserva { get; set; }
        public Actividad Actividad { get; set; }
    }
}
