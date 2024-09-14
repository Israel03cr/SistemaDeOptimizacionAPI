namespace SistemaDeOptimizacionAPI.DTOs
{
    public class RemoveUserLoginDto
    {
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }
}
