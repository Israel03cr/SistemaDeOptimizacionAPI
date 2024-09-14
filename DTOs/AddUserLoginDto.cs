namespace SistemaDeOptimizacionAPI.DTOs
{
    public class AddUserLoginDto
    {
        public string UserId { get; set; }
        public string LoginProvider { get; set; }  // Nombre del proveedor, ej: "Google", "Facebook"
        public string ProviderKey { get; set; }  // Identificador único del usuario en el proveedor externo
        public string ProviderDisplayName { get; set; }  // Nombre para mostrar del proveedor
    }
}
