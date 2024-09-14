using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SistemaDeOptimizacionAPI.Models;
using SistemaDeOptimizacionAPI.DTOs;
using System.Threading.Tasks;

namespace SistemaDeOptimizacionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTokensController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserTokensController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("getUserTokens/{userId}")]
        public async Task<IActionResult> GetUserTokens(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            // Aquí normalmente obtendrías los tokens desde una implementación custom,
            // ya que UserManager no proporciona acceso directo a los tokens.

            return Ok(new { message = "Funcionalidad para obtener tokens no implementada por defecto" });
        }

        [HttpPost("setUserToken")]
        public async Task<IActionResult> SetUserToken([FromBody] SetUserTokenDto setUserTokenDto)
        {
            var user = await _userManager.FindByIdAsync(setUserTokenDto.UserId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            // Configura el token
            var result = await _userManager.SetAuthenticationTokenAsync(user, "Default", setUserTokenDto.TokenName, setUserTokenDto.TokenValue);

            if (!result.Succeeded)
                return BadRequest(new { message = "No se pudo establecer el token." });

            return Ok(new { message = "Token actualizado exitosamente." });
        }


        [HttpDelete("removeUserToken")]
        public async Task<IActionResult> RemoveUserToken([FromBody] SetUserTokenDto setUserTokenDto)
        {
            var user = await _userManager.FindByIdAsync(setUserTokenDto.UserId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            // Aquí se eliminaría el token del usuario utilizando el TokenName y TokenValue proporcionados.
            var result = await _userManager.RemoveAuthenticationTokenAsync(user, "Default", setUserTokenDto.TokenName);
            if (!result.Succeeded)
                return BadRequest(new { message = "Error al eliminar el token" });

            return Ok(new { message = "Token eliminado exitosamente" });
        }

    }
}
