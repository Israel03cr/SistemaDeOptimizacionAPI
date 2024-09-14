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
    public class ActividadesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActividadesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todas las actividades con paginación y búsqueda
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actividad>>> GetActividades(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string nombreActividad = "")
        {
            // Creamos la consulta base
            var query = _context.Actividades.AsQueryable();

            // Filtro por nombre de la actividad si se proporciona
            if (!string.IsNullOrEmpty(nombreActividad))
            {
                query = query.Where(a => a.NombreActividad.Contains(nombreActividad));
            }

            // Paginación
            var actividades = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(actividades);
        }

        // Obtener actividad por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Actividad>> GetActividad(int id)
        {
            var actividad = await _context.Actividades.FindAsync(id);

            if (actividad == null)
            {
                return NotFound();
            }

            return Ok(actividad);
        }

        // Actualizar actividad
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActividad(int id, ActividadDto actividadDto)
        {
            if (id != actividadDto.ActividadID)
            {
                return BadRequest();
            }

            var actividad = await _context.Actividades.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }

            // Actualizar los campos de la actividad
            actividad.NombreActividad = actividadDto.NombreActividad;
            actividad.Descripcion = actividadDto.Descripcion;
      

            _context.Entry(actividad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActividadExists(id))
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

        // Registrar nueva actividad
        [HttpPost]
        public async Task<ActionResult<Actividad>> PostActividad(ActividadDto actividadDto)
        {
            var actividad = new Actividad
            {
                NombreActividad = actividadDto.NombreActividad,
                Descripcion = actividadDto.Descripcion,
                Precio = actividadDto.Precio
            };

            _context.Actividades.Add(actividad);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActividad), new { id = actividad.ActividadID }, actividad);
        }

        // Eliminar actividad
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActividad(int id)
        {
            var actividad = await _context.Actividades.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }

            _context.Actividades.Remove(actividad);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Método para verificar si una actividad existe
        private bool ActividadExists(int id)
        {
            return _context.Actividades.Any(e => e.ActividadID == id);
        }
    }
}
