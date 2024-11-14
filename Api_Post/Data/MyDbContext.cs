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
        public DbSet<Post_Evento> PostEvento { get; set; }
        public DbSet<Post_Banda> PostBanda { get; set; }
        public DbSet<Cuenta> cuenta { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Banda> Bandas { get; set; }

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
                    .WithMany(p => p.PostEventos)
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
        }
    }
}
