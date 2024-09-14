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
    public class ReservasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todas las reservas con paginación
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Reservas
                .Include(r => r.Perro)
                .Include(r => r.Servicio)
                .Include(r => r.Usuario)
                .Include(r => r.Promocion)
                .AsQueryable();

            var totalReservas = await query.CountAsync();

            var reservas = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                TotalReservas = totalReservas,
                Reservas = reservas
            });
        }

        // Obtener una reserva por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Reserva>> GetReserva(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Perro)
                .Include(r => r.Servicio)
                .Include(r => r.Usuario)
                .Include(r => r.Promocion)
                .FirstOrDefaultAsync(r => r.ReservaID == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return reserva;
        }

        // Actualizar una reserva existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReserva(int id, ReservaDto reservaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Retorna los errores de validación si existen
            }

            if (id != reservaDto.ReservaID)
            {
                return BadRequest(new { message = "El ID de la reserva no coincide" });
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound(new { message = "Reserva no encontrada" });
            }

            // Actualización de campos
            reserva.PerroID = reservaDto.PerroID;
            reserva.UsuarioID = reservaDto.UsuarioID;
            reserva.ServicioID = reservaDto.ServicioID;
            reserva.FechaReserva = reservaDto.FechaReserva;
            reserva.FechaInicio = reservaDto.FechaInicio;
            reserva.FechaFin = reservaDto.FechaFin;
            reserva.Estado = reservaDto.Estado;
            reserva.PromocionID = reservaDto.PromocionID;

            // Calcular el PrecioTotal
            await CalcularPrecioTotalAsync(reserva);

            _context.Entry(reserva).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservaExists(id))
                {
                    return NotFound(new { message = "Reserva no encontrada" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Crear una nueva reserva
        [HttpPost]
        public async Task<ActionResult<Reserva>> PostReserva(ReservaDto reservaDto)
        {
            var reserva = new Reserva
            {
                PerroID = reservaDto.PerroID,
                UsuarioID = reservaDto.UsuarioID,  // Aquí nuevamente se pasa el ID de usuario manualmente
                ServicioID = reservaDto.ServicioID,
                FechaReserva = reservaDto.FechaReserva,
                FechaInicio = reservaDto.FechaInicio,
                FechaFin = reservaDto.FechaFin,
                Estado = reservaDto.Estado,
                PromocionID = reservaDto.PromocionID
            };

            // Calcular el PrecioTotal
            await CalcularPrecioTotalAsync(reserva);

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReserva), new { id = reserva.ReservaID }, reserva);
        }

        // Eliminar una reserva
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Función para verificar si existe la reserva
        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.ReservaID == id);
        }

        // Función para calcular el PrecioTotal de la reserva
        private async Task CalcularPrecioTotalAsync(Reserva reserva)
        {
            // Obtener precio del servicio
            var precioServicio = await _context.Servicios
                .Where(s => s.ServicioID == reserva.ServicioID)
                .Select(s => s.Precio)
                .FirstOrDefaultAsync();

            // Obtener descuento de la promoción (si hay)
            var descuento = await _context.Promociones
                .Where(p => p.PromocionID == reserva.PromocionID)
                .Select(p => (decimal?)p.DescuentoPorcentaje)
                .FirstOrDefaultAsync() ?? 0m;

            // Ahora calculamos el PrecioTotal usando solo el precio del servicio
            reserva.PrecioTotal = precioServicio * (1 - descuento / 100);
        }
    }
}
