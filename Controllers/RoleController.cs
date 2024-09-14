using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SistemaDeOptimizacionAPI.DTOs;
using System.Threading.Tasks;

namespace SistemaDeOptimizacionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost("addRole")]
        public async Task<IActionResult> AddRole([FromBody] RoleDto roleDto)
        {
            if (string.IsNullOrWhiteSpace(roleDto.RoleName))
                return BadRequest(new { message = "El nombre del rol no puede estar vacío." });

            var roleExists = await _roleManager.RoleExistsAsync(roleDto.RoleName);
            if (roleExists)
                return BadRequest(new { message = "El rol ya existe." });

            var result = await _roleManager.CreateAsync(new IdentityRole(roleDto.RoleName));
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Rol creado exitosamente." });
        }

        [HttpGet("getAllRoles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles;
            return Ok(roles);
        }

        [HttpDelete("deleteRole")]
        public async Task<IActionResult> DeleteRole([FromBody] RoleDto roleDto)
        {
            var role = await _roleManager.FindByNameAsync(roleDto.RoleName);
            if (role == null)
                return NotFound(new { message = "Rol no encontrado." });

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Rol eliminado exitosamente." });
        }
    }
}
