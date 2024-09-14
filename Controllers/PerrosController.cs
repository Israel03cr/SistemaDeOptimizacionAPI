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
    public class PerrosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PerrosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PerroListDto>>> GetPerros([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var perros = await _context.Perros
                .Include(p => p.Dueño)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PerroListDto
                {
                    PerroID = p.PerroID,
                    Nombre = p.Nombre,
                    Raza = p.Raza,
                    FechaNacimiento=p.FechaNacimiento,
                    Peso = p.Peso,
                    Tamaño = p.Tamaño,
                    Sexo = p.Sexo,
                    DueñoNombre = p.Dueño.Nombre + " " + p.Dueño.Apellido
                })
                .ToListAsync();

            return Ok(perros);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Perro>> GetPerro(int id)
        {
            var perro = await _context.Perros.Include(p => p.Dueño).FirstOrDefaultAsync(p => p.PerroID == id);

            if (perro == null)
            {
                return NotFound();
            }

            return perro;
        }
        // Endpoint para obtener los perros con sus respectivos dueños
        [HttpGet("GetPerrosConDueños")]
        public async Task<ActionResult<IEnumerable<object>>> GetPerrosConDueños()
        {
            var perrosConDueños = await _context.Perros
                .Include(p => p.Dueño)
                .Select(p => new
                {
                    PerroID = p.PerroID,
                    NombrePerro = p.Nombre,
                    NombreDueño = p.Dueño.Nombre
                })
                .ToListAsync();

            return Ok(perrosConDueños);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerro(int id, [FromForm] PerroDto perroDto, [FromForm] IFormFile foto)
        {
            if (id != perroDto.PerroID)
            {
                return BadRequest();
            }

            var perro = await _context.Perros.FindAsync(id);
            if (perro == null)
            {
                return NotFound();
            }

            // Actualizar los campos del perro
            perro.Nombre = perroDto.Nombre;
            perro.Raza = perroDto.Raza;
            perro.FechaNacimiento = perroDto.FechaNacimiento;
            perro.Sexo = perroDto.Sexo;
            perro.Peso = perroDto.Peso;
            perro.Tamaño = perroDto.Tamaño;
            perro.DueñoID = perroDto.DueñoID;  // Asegurarse de que se actualiza el DueñoID

            // Si hay una imagen nueva, actualizarla
            if (foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await foto.CopyToAsync(memoryStream);
                    perro.Foto = memoryStream.ToArray();
                }
            }

            _context.Entry(perro).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerroExists(id))
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
        public async Task<ActionResult<Perro>> PostPerro([FromForm] PerroDto perroDto, [FromForm] IFormFile foto)
        {
            if (foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await foto.CopyToAsync(memoryStream);
                    perroDto.Foto = memoryStream.ToArray();
                }
            }

            var perro = new Perro
            {
                Nombre = perroDto.Nombre,
                Raza = perroDto.Raza,
                FechaNacimiento = perroDto.FechaNacimiento, // Maneja la fecha
                Peso = perroDto.Peso,
                Tamaño = perroDto.Tamaño,
                Sexo = perroDto.Sexo, // Maneja el sexo
                Foto = perroDto.Foto,
                DueñoID = perroDto.DueñoID
            };

            _context.Perros.Add(perro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerro), new { id = perro.PerroID }, perro);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerro(int id)
        {
            var perro = await _context.Perros.FindAsync(id);
            if (perro == null)
            {
                return NotFound();
            }

            _context.Perros.Remove(perro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PerroExists(int id)
        {
            return _context.Perros.Any(e => e.PerroID == id);
        }
    }
}
