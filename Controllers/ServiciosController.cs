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
    public class ServiciosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServiciosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Servicio>>> GetServicios([FromQuery] string nombre = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Servicios.AsQueryable();

            // Búsqueda por nombre si se proporciona
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(s => s.NombreServicio.Contains(nombre));
            }

            // Paginación
            var servicios = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(servicios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Servicio>> GetServicio(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);

            if (servicio == null)
            {
                return NotFound();
            }

            return servicio;
        }
    
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServicio(int id, ServicioDto servicioDto)
        {
            if (id != servicioDto.ServicioID)
            {
                return BadRequest();
            }

            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }

            servicio.NombreServicio = servicioDto.NombreServicio;
            servicio.Descripcion = servicioDto.Descripcion;
            servicio.Precio = servicioDto.Precio;

            _context.Entry(servicio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServicioExists(id))
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
        public async Task<ActionResult<Servicio>> PostServicio(ServicioDto servicioDto)
        {
            var servicio = new Servicio
            {
                NombreServicio = servicioDto.NombreServicio,
                Descripcion = servicioDto.Descripcion,
                Precio = servicioDto.Precio
            };

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServicio), new { id = servicio.ServicioID }, servicio);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServicio(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }

            _context.Servicios.Remove(servicio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServicioExists(int id)
        {
            return _context.Servicios.Any(e => e.ServicioID == id);
        }
    }
}
