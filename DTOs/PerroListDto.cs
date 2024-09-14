namespace SistemaDeOptimizacionAPI.DTOs
{
    public class PerroListDto
    {
        public int PerroID { get; set; }
        public string Nombre { get; set; }
        public string Raza { get; set; }

        public string Sexo { get; set; }
        public decimal Peso { get; set; }
        public string Tamaño { get; set; }
        public string DueñoNombre { get; set; }

        public DateTime FechaNacimiento { get; set; } // Nueva propiedad para manejar la fecha de nacimiento
    }
}
