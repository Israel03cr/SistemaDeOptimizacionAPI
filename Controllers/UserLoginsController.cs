using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SistemaDeOptimizacionAPI.Models;
using SistemaDeOptimizacionAPI.DTOs;
using System.Threading.Tasks;

namespace SistemaDeOptimizacionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserLoginsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpPost("addUserLogin")]
        public async Task<IActionResult> AddUserLogin([FromBody] AddUserLoginDto addUserLoginDto)
        {
            var user = await _userManager.FindByIdAsync(addUserLoginDto.UserId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var userLoginInfo = new UserLoginInfo(addUserLoginDto.LoginProvider, addUserLoginDto.ProviderKey, addUserLoginDto.ProviderDisplayName);
            var result = await _userManager.AddLoginAsync(user, userLoginInfo);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Login externo añadido exitosamente." });
        }

        [HttpGet("getUserLogins/{userId}")]
        public async Task<IActionResult> GetUserLogins(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var logins = await _userManager.GetLoginsAsync(user);
            return Ok(logins);
        }

        [HttpDelete("removeUserLogin")]
        public async Task<IActionResult> RemoveUserLogin([FromBody] RemoveUserLoginDto removeUserLoginDto)
        {
            var user = await _userManager.FindByIdAsync(removeUserLoginDto.UserId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var result = await _userManager.RemoveLoginAsync(user, removeUserLoginDto.LoginProvider, removeUserLoginDto.ProviderKey);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Login externo eliminado exitosamente" });
        }
    }
}
