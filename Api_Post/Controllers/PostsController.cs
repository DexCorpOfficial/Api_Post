using Api_Post.Data;
using Api_Post.DTOs;
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
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly MyDbContext _context;

        public PostController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Post
        [HttpGet]
        public async Task<ActionResult<List<Post>>> GetAll()
        {
            var Post = await _context.Post
                .Select(p => new
                {
                    p.ID,
                    p.Media,
                    p.Descripcion,
                    p.fecha_pub,
                    p.NUpvotes,
                    p.Activo,
                    p.discriminator
                })
                .ToListAsync();

            return Ok(Post);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Selecciona las propiedades necesarias del post con ID igual a 'id'
            var post = await _context.Post
                .Where(p => p.ID == id)
                .Select(p => new
                {
                    p.ID,
                    p.Media,
                    p.Descripcion,
                    p.fecha_pub,
                    p.NUpvotes,
                    p.Activo,
                    p.discriminator
                })
                .FirstOrDefaultAsync();

            // Si no se encuentra el post, devolver NotFound
            if (post == null)
            {
                return NotFound(new { message = "Post no encontrado" });
            }

            // Si se encuentra el post, devolverlo con un mensaje de éxito
            return Ok(post);
        }

        [HttpGet("GetAllPost_Feed")]
        public async Task<ActionResult<List<Post>>> GetAllPost_Feed()
        {
            var Post_Feed = await _context.Post
                .Where(p => p.discriminator == "Post_Feed")  // Filtrar por el discriminador
                .Select(p => new
                {
                    p.ID,
                    p.Media,
                    p.Descripcion,
                    p.fecha_pub,
                    p.NUpvotes,
                    p.Activo,
                    p.discriminator 
        })
                .ToListAsync();

            return Ok(Post_Feed);
        }


        [HttpGet("GetPost_FeedsByCuenta/{idCuenta}")]
        public async Task<IActionResult> GetPost_FeedsByCuenta(int idCuenta)
        {
            var postFeeds = await _context.Post_Feed
                .Where(pf => pf.IDdeCuenta == idCuenta)  // Filtrar por la cuenta
                .Include(pf => pf.Post)                  // Incluir la relación con 'Post'
                .Select(pf => new {
                    pf.IDdePost,
                    pf.IDdeCuenta,
                    Post = new
                    {
                        pf.Post.ID,
                        pf.Post.Descripcion,
                        pf.Post.Media,
                        pf.Post.NUpvotes
                    }
            // No incluir Cuenta si no lo necesitas
        })
                .ToListAsync();                          // Ejecutar la consulta

            // Si no se encuentran resultados, devolver un 404
            if (postFeeds == null || !postFeeds.Any())
            {
                return NotFound(new { message = "No se encontraron Post Feeds para la cuenta especificada." });
            }

            return Ok(postFeeds);  // Retornar los resultados encontrados
        }

        [HttpGet("GetAllPost_Banda")]
        public async Task<ActionResult<List<Post>>> GetAllPost_Banda()
        {
            var postBanda = await _context.Post
                .Where(p => p.discriminator == "Post_Banda")  // Filtrar por el discriminador
                .Select(p => new
                {
                    p.ID,
                    p.Media,
                    p.Descripcion,
                    p.fecha_pub,
                    p.NUpvotes,
                    p.Activo,
                    p.discriminator
                })
                .ToListAsync();

            return Ok(postBanda);
        }

        [HttpGet("GetPost_BandasByBanda/{idBanda}")]
        public async Task<IActionResult> GetPost_BandasByBanda(int idBanda)
        {
            var postBandas = await _context.PostBanda
                .Where(pb => pb.IDdeBanda == idBanda)  // Filtrar por la banda
                .Include(pb => pb.Post)                 // Incluir la relación con 'Post'
                .Select(pb => new
                {
                    pb.IDdePost,
                    pb.IDdeBanda,
                    pb.IDdeCuenta,
                    Post = new
                    {
                        pb.Post.ID,
                        pb.Post.Descripcion,
                        pb.Post.Media,
                        pb.Post.NUpvotes
                    }
                })
                .ToListAsync();

            // Si no se encuentran resultados, devolver un 404
            if (postBandas == null || !postBandas.Any())
            {
                return NotFound(new { message = "No se encontraron Post Bandas para la banda especificada." });
            }

            return Ok(postBandas);  // Retornar los resultados encontrados
        }

        [HttpGet("GetAllPost_Evento")]
        public async Task<ActionResult<List<Post>>> GetAllPost_Evento()
        {
            var postEvento = await _context.Post
                .Where(p => p.discriminator == "Post_Evento")  // Filtrar por el discriminador
                .Select(p => new
                {
                    p.ID,
                    p.Media,
                    p.Descripcion,
                    p.fecha_pub,
                    p.NUpvotes,
                    p.Activo,
                    p.discriminator
                })
                .ToListAsync();

            return Ok(postEvento);
        }

        [HttpGet("GetPost_EventosByEvento/{idEvento}")]
        public async Task<IActionResult> GetPost_EventosByEvento(int idEvento)
        {
            var postEventos = await _context.PostEvento
                .Where(pe => pe.IDdeEvento == idEvento)  // Filtrar por el evento
                .Include(pe => pe.Post)                   // Incluir la relación con 'Post'
                .Select(pe => new
                {
                    pe.IDdePost,
                    pe.IDdeEvento,
                    pe.IDdeCuenta,
                    Post = new
                    {
                        pe.Post.ID,
                        pe.Post.Descripcion,
                        pe.Post.Media,
                        pe.Post.NUpvotes
                    }
                })
                .ToListAsync();

            // Si no se encuentran resultados, devolver un 404
            if (postEventos == null || !postEventos.Any())
            {
                return NotFound(new { message = "No se encontraron Post Eventos para el evento especificado." });
            }

            return Ok(postEventos);  // Retornar los resultados encontrados
        }


        [HttpPost("createFeed")]
        public async Task<IActionResult> CreateFeed([Bind("Media,Descripcion,IDdeCuenta")] Post_CreateDTO Post_FeedDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos inválidos", errors = ModelState });
            }

            // 1. Crear el post base (Post) con los datos del DTO
            var newPost = new Post
            {
                Media = Post_FeedDTO.Media,
                Descripcion = Post_FeedDTO.Descripcion,
                NUpvotes = 0,
                Activo = true,
                discriminator = "Post_Feed"
            };

            _context.Post.Add(newPost);
            await _context.SaveChangesAsync();  // Guardar el Post y obtener su ID

            // 2. Crear el Post_Feed y asignar el ID de cuenta recibido del DTO
            var Post_Feed = new Post_Feed
            {
                IDdePost = newPost.ID,  // Asociar con el Post recién creado
                IDdeCuenta = Post_FeedDTO.IDdeCuenta  // Usar el ID de cuenta del DTO
            };

            // 3. Agregar el Post_Feed a la base de datos
            _context.Post_Feed.Add(Post_Feed);

            await _context.SaveChangesAsync();  // Guardar los cambios en Post_Feed

            return Ok(new { message = "Post Feed creado exitosamente", data = newPost });
        }


        [HttpPost("createBanda")]
        public async Task<IActionResult> CreateBanda([Bind("Media,Descripcion,IDdeCuenta,IDdeBanda")] Post_CreateDTO postBandaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos inválidos", errors = ModelState });
            }

            // 1. Crear el post base (Post) con los datos del DTO
            var newPost = new Post
            {
                Media = postBandaDTO.Media,
                Descripcion = postBandaDTO.Descripcion,
                NUpvotes = 0,
                Activo = true,
                discriminator = "Post_Banda"
            };

            _context.Post.Add(newPost);
            await _context.SaveChangesAsync();  // Guardar el Post y obtener su ID

            // 2. Crear el Post_Banda y asociar el ID de Banda y de cuenta
            var postBanda = new Post_Banda
            {
                IDdePost = newPost.ID,  // Asociar con el Post recién creado
                IDdeBanda = postBandaDTO.IDdeBanda.Value,  // Usar el ID de banda
                IDdeCuenta = postBandaDTO.IDdeCuenta  // Usar el ID de cuenta
            };

            // 3. Agregar el Post_Banda a la base de datos
            _context.PostBanda.Add(postBanda);

            await _context.SaveChangesAsync();  // Guardar los cambios en Post_Banda

            return Ok(new { message = "Post Banda creado exitosamente", data = newPost });
        }


        [HttpPost("createEvento")]
        public async Task<IActionResult> CreateEvento([Bind("Media,Descripcion,IDdeCuenta,IDdeEvento")] Post_CreateDTO postEventoDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos inválidos", errors = ModelState });
            }

            // 1. Crear el post base (Post) con los datos del DTO
            var newPost = new Post
            {
                Media = postEventoDTO.Media,
                Descripcion = postEventoDTO.Descripcion,
                NUpvotes = 0,
                Activo = true,
                discriminator = "Post_Evento"
            };

            _context.Post.Add(newPost);
            await _context.SaveChangesAsync();  // Guardar el Post y obtener su ID

            // 2. Crear el Post_Evento y asociar el ID de Evento y de cuenta
            var postEvento = new Post_Evento
            {
                IDdePost = newPost.ID,  // Asociar con el Post recién creado
                IDdeEvento = postEventoDTO.IDdeEvento.Value,  // Usar el ID de evento
                IDdeCuenta = postEventoDTO.IDdeCuenta  // Usar el ID de cuenta
            };

            // 3. Agregar el Post_Evento a la base de datos
            _context.PostEvento.Add(postEvento);

            await _context.SaveChangesAsync();  // Guardar los cambios en Post_Evento

            return Ok(new { message = "Post Evento creado exitosamente", data = newPost });
        }







        // PUT: api/Post/edit/5
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("ID, Media, Descripcion, fecha_pub, NUpvotes, Activo, discriminator")] Post postActualizado)
        {
            if (id != postActualizado.ID)
            {
                return BadRequest(new { message = "El ID del post no coincide con el proporcionado" });
            }

            var postExistente = await _context.Post.FindAsync(id);
            if (postExistente == null)
            {
                return NotFound(new { message = "Post no encontrado" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    postExistente.Media = postActualizado.Media;
                    postExistente.Descripcion = postActualizado.Descripcion;
                    postExistente.fecha_pub = postActualizado.fecha_pub;
                    postExistente.NUpvotes = postActualizado.NUpvotes;
                    postExistente.Activo = postActualizado.Activo;

                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Post actualizado exitosamente", data = postExistente });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return StatusCode(500, new { message = "Error al actualizar el post" });
                }
            }

            return BadRequest(new { message = "Datos del post inválidos", errors = ModelState });
        }

        // DELETE: api/Post/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound(new { message = "Post no encontrado" });
            }

            _context.Post.Remove(post);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Post eliminado exitosamente" });
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.ID == id);
        }
    }
}
