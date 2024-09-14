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
    public class PromocionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PromocionesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promocion>>> GetPromociones(
         [FromQuery] string codigo = "", // Búsqueda por código promocional
         [FromQuery] bool? activo = null, // Filtrado por estado (activo/inactivo)
        [FromQuery] int pageNumber = 1,  // Número de página
        [FromQuery] int pageSize = 10)   // Tamaño de página
        {
            // Creamos la consulta base
            var query = _context.Promociones.AsQueryable();

            // Búsqueda por código promocional
            if (!string.IsNullOrEmpty(codigo))
            {
                query = query.Where(p => p.CodigoPromocional.Contains(codigo));
            }

            // Filtrado por estado (activo/inactivo)
            if (activo.HasValue)
            {
                query = query.Where(p => p.Activo == activo.Value);
            }

            // Paginación
            var promociones = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(promociones);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Promocion>> GetPromocion(int id)
        {
            var promocion = await _context.Promociones.FindAsync(id);

            if (promocion == null)
            {
                return NotFound();
            }

            return promocion;
        }

   
        [HttpPost]
        public async Task<ActionResult<Promocion>> PostPromocion([FromBody] PromocionDto promocionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var promocion = new Promocion
            {
                CodigoPromocional = promocionDto.CodigoPromocional,
                Descripcion = promocionDto.Descripcion,
                DescuentoPorcentaje = promocionDto.DescuentoPorcentaje,
                FechaInicio = promocionDto.FechaInicio,
                FechaFin = promocionDto.FechaFin
            };

            // Calcular el estado de la promoción en base a las fechas
            ActualizarEstadoPromocion(promocion);

            _context.Promociones.Add(promocion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPromocion), new { id = promocion.PromocionID }, promocion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPromocion(int id, [FromBody] PromocionDto promocionDto)
        {
            if (id != promocionDto.PromocionID)
            {
                return BadRequest("El ID de la URL no coincide con el ID del cuerpo de la solicitud.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var promocion = await _context.Promociones.FindAsync(id);
            if (promocion == null)
            {
                return NotFound();
            }

            promocion.CodigoPromocional = promocionDto.CodigoPromocional;
            promocion.Descripcion = promocionDto.Descripcion;
            promocion.DescuentoPorcentaje = promocionDto.DescuentoPorcentaje;
            promocion.FechaInicio = promocionDto.FechaInicio;
            promocion.FechaFin = promocionDto.FechaFin;

            // Calcular el estado de la promoción en base a las fechas
            ActualizarEstadoPromocion(promocion);

            _context.Entry(promocion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PromocionExists(id))
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


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromocion(int id)
        {
            var promocion = await _context.Promociones.FindAsync(id);
            if (promocion == null)
            {
                return NotFound();
            }

            _context.Promociones.Remove(promocion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PromocionExists(int id)
        {
            return _context.Promociones.Any(e => e.PromocionID == id);
        }
        private void ActualizarEstadoPromocion(Promocion promocion)
        {
            var fechaActual = DateTime.UtcNow;

            // Activar si la fecha actual está dentro del rango de fechas
            if (fechaActual >= promocion.FechaInicio && fechaActual <= promocion.FechaFin)
            {
                promocion.Activo = true;  // La promoción está activa si hoy está dentro del rango
            }
            else
            {
                promocion.Activo = false;  // Si no está en el rango, la promoción está inactiva
            }
        }


    }
}
