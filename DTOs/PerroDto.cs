namespace SistemaDeOptimizacionAPI.DTOs
{
    public class PerroDto
    {
        public int PerroID { get; set; }
        public string Nombre { get; set; }
        public string Raza { get; set; }
        public DateTime FechaNacimiento { get; set; } // Cambiado a DateTime
        public decimal Peso { get; set; }
        public string Tamaño { get; set; }
        public byte[]? Foto { get; set; }
        public string Sexo { get; set; } // Nuevo campo para el sexo
        public int DueñoID { get; set; }
    }
}
