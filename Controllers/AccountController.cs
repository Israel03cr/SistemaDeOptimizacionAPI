using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SistemaDeOptimizacionAPI.Models;
using SistemaDeOptimizacionAPI.DTOs;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SistemaDeOptimizacionAPI.Data;

namespace SistemaDeOptimizacionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
       
        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
                return BadRequest(new { message = "El usuario ya existe" });

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                Nombre = registerDto.Nombre,
                Apellido = registerDto.Apellido,
                Telefono = registerDto.Telefono,
                Direccion = registerDto.Direccion
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Usuario registrado exitosamente" });
        }
        [HttpDelete("deleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Usuario eliminado exitosamente." });
        }
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users;

            var userList = await users.Select(user => new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.Nombre,
                user.Apellido,
                user.Direccion,
                user.Telefono
            }).ToListAsync();

            return Ok(userList);
        }


        [HttpPut("updateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(updateUserDto.UserId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            user.Nombre = updateUserDto.Nombre;
            user.Apellido = updateUserDto.Apellido;
            user.Email = updateUserDto.Email;
            user.UserName = updateUserDto.Email;  // Asegúrate de actualizar también el UserName si cambia el email
            user.Direccion = updateUserDto.Direccion;
            user.Telefono = updateUserDto.Telefono;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Usuario actualizado exitosamente." });
        }
   
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
                return Unauthorized(new { message = "Credenciales inválidas" });

            var authClaims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
    }
}
