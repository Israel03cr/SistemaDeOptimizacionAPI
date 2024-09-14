namespace SistemaDeOptimizacionAPI.Models
{
    public class Perro
    {
        public int PerroID { get; set; }
        public string Nombre { get; set; }
        public string Raza { get; set; }
        public DateTime FechaNacimiento { get; set; } // Cambia Edad a tipo fecha
        public decimal Peso { get; set; }
        public string Tamaño { get; set; }
        public byte[] Foto { get; set; }

        public string Sexo { get; set; } // Nuevo campo para el sexo (hembra o macho)

        public Dueño Dueño { get; set; }
        public int DueñoID { get; set; }

        public ICollection<Reserva> Reservas { get; set; }
    }
}
