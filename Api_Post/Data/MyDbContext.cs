using Api_Post.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Post.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        // Definir las tablas para las entidades
        public DbSet<Post> Post { get; set; }
        public DbSet<Post_Feed> Post_Feed { get; set; }
        public DbSet<Post_Evento> Post_Evento { get; set; }
        public DbSet<Post_Banda> Post_Banda { get; set; }
        public DbSet<Cuenta> Cuenta { get; set; }
        public DbSet<Evento> Evento { get; set; }
        public DbSet<Banda> Bandas { get; set; }
        public DbSet<Likea> Likea { get; set; }
        public DbSet<Comentario> Comentario { get; set; }  // DbSet de Comentario
        public DbSet<Responde> Responde { get; set; }  // DbSet de Responde

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad 'Post'
            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");
                entity.HasKey(p => p.ID);  // Definir la clave primaria
            });

            // Configuración de la entidad 'Post_Feed'
            modelBuilder.Entity<Post_Feed>(entity =>
            {
                entity.HasKey(pf => pf.IDdePost);  // Clave primaria

                // Relación con 'Post'
                entity.HasOne(pf => pf.Post)
                    .WithMany(p => p.Post_Feeds)
                    .HasForeignKey(pf => pf.IDdePost)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con 'Cuenta'
                entity.HasOne(pf => pf.Cuenta)
                    .WithMany()
                    .HasForeignKey(pf => pf.IDdeCuenta)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de la entidad 'Post_Evento'
            modelBuilder.Entity<Post_Evento>(entity =>
            {
                entity.HasKey(pe => pe.IDdePost);  // Clave primaria

                // Relación con 'Post'
                entity.HasOne(pe => pe.Post)
                    .WithMany(p => p.PostEvento)
                    .HasForeignKey(pe => pe.IDdePost)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con 'Cuenta'
                entity.HasOne(pe => pe.Cuenta)
                    .WithMany()
                    .HasForeignKey(pe => pe.IDdeCuenta)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con 'Evento'
                entity.HasOne(pe => pe.Evento)
                    .WithMany()
                    .HasForeignKey(pe => pe.IDdeEvento)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de la entidad 'Post_Banda'
            modelBuilder.Entity<Post_Banda>(entity =>
            {
                entity.HasKey(pb => pb.IDdePost);  // Clave primaria

                // Relación con 'Post'
                entity.HasOne(pb => pb.Post)
                    .WithMany(p => p.PostBandas)
                    .HasForeignKey(pb => pb.IDdePost)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con 'Cuenta'
                entity.HasOne(pb => pb.Cuenta)
                    .WithMany()
                    .HasForeignKey(pb => pb.IDdeCuenta)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con 'Banda'
                entity.HasOne(pb => pb.Banda)
                    .WithMany()
                    .HasForeignKey(pb => pb.IDdeBanda)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de la entidad 'Likea'
            modelBuilder.Entity<Likea>(entity =>
            {
                // Clave primaria compuesta
                entity.HasKey(l => new { l.IDdePost, l.IDdeCuenta });

                // Relación con 'Post'
                entity.HasOne(l => l.Post)
                    .WithMany() // No hay propiedad de navegación en 'Post' para Likea
                    .HasForeignKey(l => l.IDdePost)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con 'Cuenta'
                entity.HasOne(l => l.Cuenta)
                    .WithMany() // No hay propiedad de navegación en 'Cuenta' para Likea
                    .HasForeignKey(l => l.IDdeCuenta)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de la entidad 'Comentario'
            modelBuilder.Entity<Comentario>(entity =>
            {
                entity.HasKey(c => c.ID);  // Clave primaria

                // Relación con 'Post'
                entity.HasOne(c => c.Post)
                    .WithMany()  // No hay propiedad de navegación en 'Post' para Comentario
                    .HasForeignKey(c => c.IDdePost)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con 'Cuenta'
                entity.HasOne(c => c.Cuenta)
                    .WithMany()  // No hay propiedad de navegación en 'Cuenta' para Comentario
                    .HasForeignKey(c => c.IDdeCuenta)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de la entidad 'Responde' para la relación entre comentarios
            modelBuilder.Entity<Responde>(entity =>
            {
                // Definir la clave primaria compuesta por 'IDdePadre' e 'IDdeHijo'
                entity.HasKey(r => new { r.IDdePadre, r.IDdeHijo });

                // Relación con 'Comentario' (Padre)
                entity.HasOne(r => r.Padre)
                    .WithMany()  // No hay propiedad de navegación en 'Comentario' para Responde
                    .HasForeignKey(r => r.IDdePadre)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con 'Comentario' (Hijo)
                entity.HasOne(r => r.Hijo)
                    .WithMany()  // No hay propiedad de navegación en 'Comentario' para Responde
                    .HasForeignKey(r => r.IDdeHijo)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Cuenta>()
                .HasMany(c => c.Eventos)  // Navegación inversa
                .WithOne(e => e.Cuenta)   // Relación en Evento
                .HasForeignKey(e => e.IDdeCuenta)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
