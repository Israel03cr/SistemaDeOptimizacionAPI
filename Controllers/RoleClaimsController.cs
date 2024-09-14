using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SistemaDeOptimizacionAPI.DTOs;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SistemaDeOptimizacionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleClaimsController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleClaimsController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost("addRoleClaim")]
        public async Task<IActionResult> AddClaimToRole([FromBody] AddRoleClaimDto addRoleClaimDto)
        {
            var role = await _roleManager.FindByIdAsync(addRoleClaimDto.RoleId);
            if (role == null)
                return NotFound(new { message = "Rol no encontrado" });

            var claim = new Claim(addRoleClaimDto.ClaimType, addRoleClaimDto.ClaimValue);
            var result = await _roleManager.AddClaimAsync(role, claim);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Claim añadida al rol exitosamente" });
        }

        [HttpGet("getRoleClaims/{roleId}")]
        public async Task<IActionResult> GetRoleClaims(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound(new { message = "Rol no encontrado" });

            var claims = await _roleManager.GetClaimsAsync(role);
            return Ok(claims);
        }

        [HttpDelete("removeRoleClaim")]
        public async Task<IActionResult> RemoveClaimFromRole([FromBody] AddRoleClaimDto removeRoleClaimDto)
        {
            var role = await _roleManager.FindByIdAsync(removeRoleClaimDto.RoleId);
            if (role == null)
                return NotFound(new { message = "Rol no encontrado" });

            var claim = new Claim(removeRoleClaimDto.ClaimType, removeRoleClaimDto.ClaimValue);  // Usa el ClaimValue proporcionado
            var result = await _roleManager.RemoveClaimAsync(role, claim);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Claim eliminada del rol exitosamente" });
        }

    }
}
