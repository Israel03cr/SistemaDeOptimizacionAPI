namespace SistemaDeOptimizacionAPI.DTOs
{
    public class UpdateUserDto
    {
        public string UserId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
    }
}
