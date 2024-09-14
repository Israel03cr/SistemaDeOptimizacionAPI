using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SistemaDeOptimizacionAPI.Models;
using SistemaDeOptimizacionAPI.DTOs;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SistemaDeOptimizacionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserClaimsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserClaimsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("addClaim")]
        public async Task<IActionResult> AddClaimToUser([FromBody] AddClaimDto addClaimDto)
        {
            var user = await _userManager.FindByIdAsync(addClaimDto.UserId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var claim = new Claim(addClaimDto.ClaimType, addClaimDto.ClaimValue);
            var result = await _userManager.AddClaimAsync(user, claim);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Claim añadida al usuario exitosamente" });
        }

        [HttpGet("getUserClaims/{userId}")]
        public async Task<IActionResult> GetUserClaims(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var claims = await _userManager.GetClaimsAsync(user);
            var simplifiedClaims = claims.Select(c => new { c.Type, c.Value }).ToList();

            return Ok(simplifiedClaims);
        }


        [HttpDelete("removeClaim")]
        public async Task<IActionResult> RemoveClaimFromUser([FromBody] RemoveClaimDto removeClaimDto)
        {
            var user = await _userManager.FindByIdAsync(removeClaimDto.UserId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var claim = new Claim(removeClaimDto.ClaimType, string.Empty);
            var result = await _userManager.RemoveClaimAsync(user, claim);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Claim eliminada del usuario exitosamente" });
        }
    }
}
