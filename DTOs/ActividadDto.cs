namespace SistemaDeOptimizacionAPI.DTOs
{
    public class ActividadDto
    {
        public int ActividadID { get; set; }
        public string NombreActividad { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; } // Puedesel campo aunque no lo uses en el cálculo mantener 
    }
}
