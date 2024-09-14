namespace SistemaDeOptimizacionAPI.Models
{
    public class Actividad
    {
        public int ActividadID { get; set; }
        public string NombreActividad { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; } // Puedes mantener el campo aunque no lo uses en el cálculo

        public ICollection<ControlActividad> ControlActividades { get; set; }
    }
}
