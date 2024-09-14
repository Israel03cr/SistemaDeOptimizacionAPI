using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SistemaDeOptimizacionAPI.DTOs;
using SistemaDeOptimizacionAPI.Models;
using System.Threading.Tasks;

namespace SistemaDeOptimizacionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("assignRoleToUser")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleDto assignRoleDto)
        {
            var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var roleExists = await _roleManager.RoleExistsAsync(assignRoleDto.RoleName);
            if (!roleExists)
                return NotFound(new { message = "Rol no encontrado" });

            var result = await _userManager.AddToRoleAsync(user, assignRoleDto.RoleName);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Rol asignado exitosamente al usuario." });
        }
        [HttpGet("getUserRoles/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }
        [HttpDelete("removeRoleFromUser")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] AssignRoleDto assignRoleDto)
        {
            var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var result = await _userManager.RemoveFromRoleAsync(user, assignRoleDto.RoleName);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Rol eliminado del usuario exitosamente." });
        }


    }
}
