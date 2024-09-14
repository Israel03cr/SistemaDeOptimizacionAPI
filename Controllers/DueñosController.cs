using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeOptimizacionAPI.Data;
using SistemaDeOptimizacionAPI.DTOs;
using SistemaDeOptimizacionAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeOptimizacionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DueñosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DueñosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DueñoDto>>> GetDueños([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var dueños = await _context.Dueños
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DueñoDto
                {
                    DueñoID = d.DueñoID,
                    Nombre = d.Nombre,
                    Apellido = d.Apellido,
                    Direccion = d.Direccion,
                    Telefono = d.Telefono,
                    Email = d.Email
                })
                .ToListAsync();

            return Ok(dueños);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Dueño>> GetDueño(int id)
        {
            var dueño = await _context.Dueños.Include(d => d.Perros).FirstOrDefaultAsync(d => d.DueñoID == id);

            if (dueño == null)
            {
                return NotFound();
            }

            return dueño;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDueño(int id, DueñoDto dueñoDto)
        {
            if (id != dueñoDto.DueñoID)
            {
                return BadRequest();
            }

            var dueño = await _context.Dueños.FindAsync(id);
            if (dueño == null)
            {
                return NotFound();
            }

            dueño.Nombre = dueñoDto.Nombre;
            dueño.Apellido = dueñoDto.Apellido;
            dueño.Direccion = dueñoDto.Direccion;
            dueño.Telefono = dueñoDto.Telefono;
            dueño.Email = dueñoDto.Email;

            _context.Entry(dueño).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DueñoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Dueño>> PostDueño(DueñoDto dueñoDto)
        {
            var dueño = new Dueño
            {
                Nombre = dueñoDto.Nombre,
                Apellido = dueñoDto.Apellido,
                Direccion = dueñoDto.Direccion,
                Telefono = dueñoDto.Telefono,
                Email = dueñoDto.Email
            };

            _context.Dueños.Add(dueño);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDueño), new { id = dueño.DueñoID }, dueño);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDueño(int id)
        {
            var dueño = await _context.Dueños.FindAsync(id);
            if (dueño == null)
            {
                return NotFound();
            }

            _context.Dueños.Remove(dueño);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DueñoExists(int id)
        {
            return _context.Dueños.Any(e => e.DueñoID == id);
        }
    }
}
