using Api_Post.Data;
using Api_Post.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Post.Controllers
{
    [ApiController]
    [Route("Comentario")]
    public class ComentarioController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ComentarioController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Comentario
        [HttpGet]
        public async Task<ActionResult<List<Comentario>>> GetAll()
        {
            return await _context.Comentario.Include(c => c.Cuenta).Include(c => c.Post).ToListAsync();
        }

        // GET: api/Comentario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comentario>> GetById(int id)
        {
            try
            {
                Console.WriteLine($"Intentando obtener el comentario con ID: {id}");

                var comentario = await _context.Comentario
                    .Include(c => c.Cuenta)
                    .Include(c => c.Post)
                    .FirstOrDefaultAsync(c => c.ID == id);

                if (comentario == null)
                {
                    Console.WriteLine($"Comentario con ID {id} no encontrado.");
                    return NotFound($"Comentario con ID {id} no encontrado.");
                }

                Console.WriteLine($"Comentario obtenido: {comentario.Contenido}, cuenta asociada: {comentario.Cuenta?.Contrasenia ?? "No encontrada"}");

                return Ok(comentario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        // GET: Comentario/post/{idPost}
        [HttpGet("post/{idPost}")]
        public async Task<ActionResult<List<Comentario>>> GetByPostId(int idPost)
        {
            try
            {
                Console.WriteLine($"Intentando obtener los comentarios para el post con ID: {idPost}");

                // Buscar todos los comentarios asociados a un post específico
                var comentarios = await _context.Comentario
                    .Include(c => c.Cuenta)  // Incluir la relación con la cuenta (usuario)
                    .Include(c => c.Post)    // Incluir la relación con el post
                    .Where(c => c.IDdePost == idPost)  // Filtrar por el ID del post
                    .ToListAsync();

                if (comentarios == null || comentarios.Count == 0)
                {
                    Console.WriteLine($"No se encontraron comentarios para el post con ID {idPost}.");
                    return NotFound($"No se encontraron comentarios para el post con ID {idPost}.");
                }

                Console.WriteLine($"Comentarios obtenidos para el post con ID {idPost}: {comentarios.Count}");

                return Ok(comentarios);  // Retornar los comentarios encontrados
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener comentarios: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "Error interno del servidor.");
            }
        }


        // POST: api/Comentario/create
        [HttpPost("create")]
        public async Task<ActionResult<Comentario>> Create([Bind("ID, IDdePost, IDdeCuenta, Contenido, Fecha, Activo")] Comentario comentario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Insertar el comentario en la base de datos
                    _context.Comentario.Add(comentario);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetById), new { id = comentario.ID }, comentario);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error en el servidor: {ex.Message}");
                }
            }

            return BadRequest(ModelState);
        }

        // PUT: api/Comentario/edit/5
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("ID, IDdePost, IDdeCuenta, Contenido, Fecha, Activo")] Comentario comentario)
        {
            if (id != comentario.ID)
            {
                return BadRequest();
            }

            var comentarioExistente = await _context.Comentario.FindAsync(id);
            if (comentarioExistente == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizando los valores del comentario
                    comentarioExistente.Contenido = comentario.Contenido;
                    comentarioExistente.Fecha = comentario.Fecha;
                    comentarioExistente.Activo = comentario.Activo;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComentarioExists(comentarioExistente.ID))
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

        // DELETE: api/Comentario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var comentario = await _context.Comentario.FindAsync(id);
            if (comentario == null)
            {
                return NotFound();
            }

            // Cambiar el estado del comentario a inactivo en lugar de eliminarlo
            comentario.Activo = false;
            _context.Comentario.Update(comentario);
            await _context.SaveChangesAsync();

            return NoContent();  // Indica que la eliminación (en este caso, desactivación) fue exitosa
        }

        private bool ComentarioExists(int id)
        {
            return _context.Comentario.Any(c => c.ID == id);
        }

        [HttpPost("crearRespuesta")]
        public async Task<ActionResult> CrearRespuesta([FromBody] Comentario comentarioRespuesta, [FromQuery] int idDePadre)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar que el comentario padre exista
                    var comentarioPadre = await _context.Comentario.FindAsync(idDePadre);
                    if (comentarioPadre == null)
                    {
                        return NotFound($"Comentario padre con ID {idDePadre} no encontrado.");
                    }

                    // Crear el nuevo comentario (respuesta)
                    comentarioRespuesta.Fecha = DateTime.Now;  // Asignar la fecha actual
                    comentarioRespuesta.Activo = true;         // Asignar el estado como activo
                    _context.Comentario.Add(comentarioRespuesta);
                    await _context.SaveChangesAsync();  // Guardar el comentario en la base de datos

                    // Crear la relación en la tabla 'Responde'
                    var responde = new Responde
                    {
                        IDdePadre = idDePadre,             // Comentario padre
                        IDdeHijo = comentarioRespuesta.ID  // El ID del comentario recién creado
                    };

                    _context.Responde.Add(responde);  // Agregar la relación en 'Responde'
                    await _context.SaveChangesAsync();  // Guardar la relación en la base de datos

                    // Retornar la respuesta exitosa
                    return CreatedAtAction(nameof(GetById), new { id = comentarioRespuesta.ID }, comentarioRespuesta);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al crear la respuesta: {ex.Message}");
                    return StatusCode(500, $"Error en el servidor: {ex.Message}");
                }
            }

            return BadRequest(ModelState);
        }







    }
}
