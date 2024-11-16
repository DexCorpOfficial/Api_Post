using Api_Post.Data;
using Api_Post.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Post.Controllers
{
    [ApiController]
    [Route("Evento")]
    public class EventoController : ControllerBase
    {
        private readonly MyDbContext _context;

        public EventoController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Evento
        [HttpGet]
        public async Task<ActionResult<List<Evento>>> GetAll()
        {
            return await _context.Evento.ToListAsync();
        }

        // GET: Evento/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Evento>> GetById(int id)
        {
            try
            {
                Console.WriteLine($"Intentando obtener el evento con ID: {id}");

                // Incluir la entidad relacionada 'Cuenta'
                var evento = await _context.Evento
                    .Include(e => e.Cuenta)
                    .FirstOrDefaultAsync(e => e.ID == id);

                if (evento == null)
                {
                    Console.WriteLine($"Evento con ID {id} no encontrado.");
                    return NotFound($"Evento con ID {id} no encontrado.");
                }

                Console.WriteLine($"Evento obtenido: {evento.Nombre}, cuenta asociada: {evento.Cuenta?.Nombre ?? "No encontrada"}");

                return Ok(evento);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "Error interno del servidor.");
            }
        }



        [HttpPost("create")]
        public async Task<ActionResult<Evento>> Create([Bind("ID, IDdeCuenta, fecha_ini, fecha_fin, Nombre, Descripcion, Activo")] Evento evento)
        {
            if (ModelState.IsValid)
            {
                // Realizar la inserción usando una consulta SQL "raw"
                var sql = @"INSERT INTO Evento (IDdeCuenta, fecha_ini, fecha_fin, Nombre, Descripcion, Activo) 
                    VALUES (@IDdeCuenta, @fecha_ini, @fecha_fin, @Nombre, @Descripcion, @Activo)";

                var parameters = new[]
                {
            new MySqlParameter("@IDdeCuenta", MySqlDbType.Int32) { Value = evento.IDdeCuenta },
            new MySqlParameter("@fecha_ini", MySqlDbType.DateTime) { Value = evento.fecha_ini },
            new MySqlParameter("@fecha_fin", MySqlDbType.DateTime) { Value = evento.fecha_fin },
            new MySqlParameter("@Nombre", MySqlDbType.VarChar) { Value = evento.Nombre },
            new MySqlParameter("@Descripcion", MySqlDbType.VarChar) { Value = evento.Descripcion ?? (object)DBNull.Value },
            new MySqlParameter("@Activo", MySqlDbType.Bit) { Value = evento.Activo }
        };

                try
                {
                    // Ejecutar la consulta de inserción
                    await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                    // Recuperar el ID generado utilizando una consulta SQL que devuelve el ID
                    var newId = await _context.Evento
                        .FromSqlRaw("SELECT LAST_INSERT_ID() AS ID")
                        .Select(e => e.ID)  // Asegúrate de usar 'ID' en mayúsculas aquí
                        .FirstOrDefaultAsync();

                    // Asignar el ID generado automáticamente al objeto evento
                    evento.ID = newId;

                    return CreatedAtAction(nameof(GetById), new { id = evento.ID }, evento);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error en el servidor: {ex.Message}");
                }
            }

            return BadRequest(ModelState);
        }



        [HttpGet("GetEventosActivos")]
        public async Task<ActionResult<IEnumerable<Evento>>> GetEventosActivos()
        {
            DateTime fechaActual = DateTime.Now;

            // Filtrar eventos activos y con fecha_fin mayor a la actual
            var eventos = await _context.Evento
                .Where(e => e.Activo && e.fecha_fin > fechaActual)
                .Select(e => new
                {
                    e.ID,
                    e.Nombre,
                    fecha_ini = e.fecha_ini,
                    fecha_fin = e.fecha_fin
                })
                .ToListAsync();

            return Ok(eventos);
        }



        // PUT: api/Evento/edit/5
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("ID, IDdeCuenta, fecha_ini, fecha_fin, Nombre, Descripcion, Activo")] Evento Evento)
        {
            if (id != Evento.ID)
            {
                return BadRequest();
            }

            var EventoExistente = await _context.Evento.FindAsync(id);
            if (EventoExistente == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizando los valores del Evento
                    EventoExistente.fecha_ini = Evento.fecha_ini;
                    EventoExistente.fecha_fin = Evento.fecha_fin;
                    EventoExistente.Nombre = Evento.Nombre;
                    EventoExistente.Descripcion = Evento.Descripcion;
                    EventoExistente.Activo = Evento.Activo;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventoExists(EventoExistente.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return NoContent();  // Indica que la actualización fue exitosa sin necesidad de respuesta adicional
            }

            return BadRequest(ModelState);
        }

        // DELETE: api/Evento/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Evento = await _context.Evento.FindAsync(id);
            if (Evento == null)
            {
                return NotFound();
            }

            // Cambiar el estado del Evento a inactivo en lugar de eliminarlo
            Evento.Activo = false;
            _context.Evento.Update(Evento);
            await _context.SaveChangesAsync();

            return NoContent();  // Indica que la eliminación (en este caso, desactivación) fue exitosa
        }

        private bool EventoExists(int id)
        {
            return _context.Evento.Any(e => e.ID == id);
        }
    }
}
