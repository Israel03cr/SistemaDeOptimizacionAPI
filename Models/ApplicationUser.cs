using Microsoft.AspNetCore.Identity;

namespace SistemaDeOptimizacionAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }

        public ICollection<Reserva> Reservas { get; set; }
    }
}
