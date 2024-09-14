using SistemaDeOptimizacionAPI.Models;
using System.Collections.Generic;

namespace SistemaDeOptimizacionAPI.DTOs
{
    public class DueñoDto
    {
        public int DueñoID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public ICollection<Perro> Perros { get; set; } = new List<Perro>();
    }
}
