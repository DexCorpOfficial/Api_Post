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


        [HttpGet("GetPost_FeedsByPost/{idPost}")]
        public async Task<IActionResult> GetPost_FeedsByPost(int idPost)
        {
            var postFeeds = await _context.Post_Feed
                .Where(pf => pf.IDdePost == idPost)  // Filtrar por el post
                .Include(pf => pf.Post)                  // Incluir la relación con 'Post'
                .Select(pf => new {
                    pf.IDdePost,
                    pf.IDdeCuenta,
                    Post = new
                    {
                        pf.Post.ID,
                        pf.Post.Descripcion,
                        pf.Post.Media,
                        pf.Post.NUpvotes,
                        pf.Post.fecha_pub
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
        [HttpGet("GetPost_BandasByPost/{idPost}")]
        public async Task<IActionResult> GetPost_BandasByPost(int idPost)
        {
            var postBandas = await _context.Post_Banda
                .Where(pb => pb.IDdePost == idPost)  // Filtrar por el post
                .Include(pb => pb.Post)              // Incluir la relación con 'Post'
                .Select(pb => new {
                    pb.IDdePost,
                    pb.IDdeBanda,
                    pb.IDdeCuenta,
                    Post = new
                    {
                        pb.Post.ID,
                        pb.Post.Descripcion,
                        pb.Post.Media,
                        pb.Post.NUpvotes,
                        pb.Post.fecha_pub
                    }
                })
                .ToListAsync();                      // Ejecutar la consulta

            // Si no se encuentran resultados, devolver un 404
            if (postBandas == null || !postBandas.Any())
            {
                return NotFound(new { message = "No se encontraron Post Bandas para el ID de post especificado." });
            }

            return Ok(postBandas);  // Retornar los resultados encontrados
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

        [HttpGet("GetAllLikea")]
        public async Task<ActionResult<List<object>>> GetAllLikea()
        {
            var likeas = await _context.Likea
                .Select(l => new
                {
                    l.IDdePost,
                    l.IDdeCuenta
                })
                .ToListAsync();

            return Ok(likeas);
        }

        [HttpGet("GetLikeasByPost/{idPost}")]
        public async Task<IActionResult> GetLikeasByPost(int idPost)
        {
            var likeas = await _context.Likea
                .Where(l => l.IDdePost == idPost)  // Filtrar por el ID de Post
                .Select(l => new
                {
                    l.IDdePost,
                    l.IDdeCuenta
                })
                .ToListAsync();  // Ejecutar la consulta

            // Si no se encuentran resultados, devolver un 404
            if (likeas == null || !likeas.Any())
            {
                return NotFound(new { message = "No se encontraron likes para el post especificado." });
            }

            return Ok(likeas);  // Retornar los resultados encontrados
        }

        [HttpPost("CreateLikea")]
        public async Task<IActionResult> CreateLikea([FromBody] Likea likea)
        {
            // Verificar si el Likea ya existe (para evitar duplicados)
            var existingLikea = await _context.Likea
                .FirstOrDefaultAsync(l => l.IDdePost == likea.IDdePost && l.IDdeCuenta == likea.IDdeCuenta);

            if (existingLikea != null)
            {
                return BadRequest(new { message = "Este Likea ya existe para el Post y Cuenta especificados." });
            }

            // Obtener el Post asociado al Likea
            var post = await _context.Post.FindAsync(likea.IDdePost);

            if (post == null)
            {
                return NotFound(new { message = "El Post no existe." });
            }

            // Aumentar el número de upvotes del Post en 1
            post.NUpvotes += 1;

            // Agregar el nuevo Likea
            _context.Likea.Add(likea);

            try
            {
                // Guardar los cambios tanto en el Likea como en el Post
                await _context.SaveChangesAsync();

                // Retornar la respuesta con el Likea creado
                return CreatedAtAction(nameof(GetLikeasByPost), new { idPost = likea.IDdePost }, likea);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al guardar el Likea", error = ex.Message });
            }
        }

        [HttpDelete("DeleteLikea")]
        public async Task<IActionResult> DeleteLikea([FromQuery] int idPost, [FromQuery] int idCuenta)
        {
            // Buscar el registro de Likea específico con IDdePost y IDdeCuenta
            var likea = await _context.Likea
                .FirstOrDefaultAsync(l => l.IDdePost == idPost && l.IDdeCuenta == idCuenta);

            // Verificar si se encontró el registro Likea
            if (likea == null)
            {
                return NotFound(new { message = "No se encontró el Likea con los valores especificados." });
            }

            // Obtener el Post asociado
            var post = await _context.Post.FindAsync(idPost);

            // Verificar si el Post existe
            if (post == null)
            {
                return NotFound(new { message = "El Post no existe." });
            }

            // Decrementar el número de upvotes del Post en 1
            post.NUpvotes -= 1;

            // Eliminar el Likea
            _context.Likea.Remove(likea);

            try
            {
                // Guardar los cambios en la base de datos (tanto en Likea como en Post)
                await _context.SaveChangesAsync();
                return Ok(new { message = "Likea eliminado correctamente, y el número de upvotes ha sido actualizado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el Likea", error = ex.Message });
            }
        }



        [HttpGet("GetPost_BandasByBanda/{idBanda}")]
        public async Task<IActionResult> GetPost_BandasByBanda(int idBanda)
        {
            var postBandas = await _context.Post_Banda
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

        [HttpGet("GetPost_EventoByEvento/{idEvento}")]
        public async Task<IActionResult> GetPost_EventoByEvento(int idEvento)
        {
            var postEvento = await _context.Post_Evento
                .Where(pe => pe.IDdeEvento == idEvento)  // Filtrar por el Evento
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
                        pe.Post.NUpvotes,
                        pe.Post.fecha_pub
                    }
                })
                .ToListAsync();

            // Si no se encuentran resultados, devolver un 404
            if (postEvento == null || !postEvento.Any())
            {
                return NotFound(new { message = "No se encontraron Post Evento para el Evento especificado." });
            }

            return Ok(postEvento);  // Retornar los resultados encontrados
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
            _context.Post_Banda.Add(postBanda);

            await _context.SaveChangesAsync();  // Guardar los cambios en Post_Banda

            return Ok(new { message = "Post Banda creado exitosamente", data = new { id = newPost.ID } });

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
                IDdeEvento = postEventoDTO.IDdeEvento.Value,  // Usar el ID de Evento
                IDdeCuenta = postEventoDTO.IDdeCuenta  // Usar el ID de cuenta
            };

            // 3. Agregar el Post_Evento a la base de datos
            _context.Post_Evento.Add(postEvento);

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
