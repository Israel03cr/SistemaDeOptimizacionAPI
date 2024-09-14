using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaDeOptimizacionAPI.Models
{
    public class Dueño
    {
        public int DueñoID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

        public ICollection<Perro> Perros { get; set; }
    }
}
