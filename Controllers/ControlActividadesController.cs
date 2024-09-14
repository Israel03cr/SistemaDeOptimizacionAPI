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
    public class ControlActividadesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ControlActividadesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ControlActividadDto>>> GetControlActividades()
        {
            var actividades = await _context.ControlActividades
                .Include(ca => ca.Reserva)
                    .ThenInclude(r => r.Perro)
                    .ThenInclude(p => p.Dueño)
                .Include(ca => ca.Actividad)
                .Select(ca => new ControlActividadDto
                {
                    ControlID = ca.ControlID,
                    ReservaID = ca.ReservaID,
                    NombrePerro = ca.Reserva.Perro.Nombre,
                    NombreDueño = ca.Reserva.Perro.Dueño.Nombre,
                    ActividadID = ca.ActividadID,
                    NombreActividad = ca.Actividad.NombreActividad,
                    FechaActividad = ca.FechaActividad
                })
                .ToListAsync();

            return Ok(actividades);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ControlActividad>> GetControlActividad(int id)
        {
            var controlActividad = await _context.ControlActividades
                .Include(ca => ca.Reserva)
                .Include(ca => ca.Actividad)
                .FirstOrDefaultAsync(ca => ca.ControlID == id);

            if (controlActividad == null)
            {
                return NotFound();
            }

            return controlActividad;
        }


        // Actualizar actividad
        [HttpPut("{id}")]
        public async Task<IActionResult> PutControlActividad(int id, CARegistraryActualizar controlActividadDto)
        {
            if (id != controlActividadDto.ControlID)  // Verificar que el ID en la URL coincida con el ControlID del DTO
            {
                return BadRequest("El ID de la actividad no coincide.");
            }

            var controlActividad = await _context.ControlActividades.FindAsync(id);
            if (controlActividad == null)
            {
                return NotFound();
            }

            // Actualizar los campos
            controlActividad.ReservaID = controlActividadDto.ReservaID;
            controlActividad.ActividadID = controlActividadDto.ActividadID;
            controlActividad.FechaActividad = controlActividadDto.FechaActividad;

            _context.Entry(controlActividad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ControlActividadExists(id))
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


        // Crear nueva actividad
        [HttpPost]
        public async Task<ActionResult<ControlActividad>> PostControlActividad(CARegistraryActualizar controlActividadDto)
        {
            var controlActividad = new ControlActividad
            {   
                ControlID = controlActividadDto.ControlID,
                ReservaID = controlActividadDto.ReservaID,
                ActividadID = controlActividadDto.ActividadID,
                FechaActividad = controlActividadDto.FechaActividad
            };

            _context.ControlActividades.Add(controlActividad);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetControlActividad), new { id = controlActividad.ControlID }, controlActividad);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteControlActividad(int id)
        {
            var controlActividad = await _context.ControlActividades.FindAsync(id);
            if (controlActividad == null)
            {
                return NotFound();
            }

            _context.ControlActividades.Remove(controlActividad);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ControlActividadExists(int id)
        {
            return _context.ControlActividades.Any(e => e.ControlID == id);
        }
    }
}
