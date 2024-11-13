using Api_Post.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Post.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<Cuenta> Cuenta { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Post_Feed> PostFeed { get; set; }
        public DbSet<Post_Evento> PostEvento { get; set; }
        public DbSet<Post_Banda> PostBanda { get; set; }
        public DbSet<Evento> Evento { get; set; }
        public DbSet<Banda> Banda { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de herencia: Las subclases heredan de Post
            modelBuilder.Entity<Post_Feed>().HasBaseType<Post>();
            modelBuilder.Entity<Post_Evento>().HasBaseType<Post>();
            modelBuilder.Entity<Post_Banda>().HasBaseType<Post>();

            // Relación de Post con Post_Feed (Post_Feed tiene 2 claves foráneas: Post y Cuenta)
            modelBuilder.Entity<Post_Feed>()
                .HasOne(pf => pf.Cuenta)
                .WithMany()
                .HasForeignKey(pf => pf.IDdeCuenta)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post_Feed>()
                .HasOne(pf => pf.Post)
                .WithOne()
                .HasForeignKey<Post_Feed>(pf => pf.IDdePost)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación de Post con Post_Evento (Post_Evento tiene 3 claves foráneas: Post, Cuenta, Evento)
            modelBuilder.Entity<Post_Evento>()
                .HasOne(pe => pe.Cuenta)
                .WithMany()
                .HasForeignKey(pe => pe.IDdeCuenta)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post_Evento>()
                .HasOne(pe => pe.Post)
                .WithOne()
                .HasForeignKey<Post_Evento>(pe => pe.IDdePost)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post_Evento>()
                .HasOne(pe => pe.Evento)
                .WithMany()
                .HasForeignKey(pe => pe.IDdeEvento)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación de Post con Post_Banda (Post_Banda tiene 3 claves foráneas: Post, Cuenta, Banda)
            modelBuilder.Entity<Post_Banda>()
                .HasOne(pb => pb.Cuenta)
                .WithMany()
                .HasForeignKey(pb => pb.IDdeCuenta)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post_Banda>()
                .HasOne(pb => pb.Post)
                .WithOne()
                .HasForeignKey<Post_Banda>(pb => pb.IDdePost)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post_Banda>()
                .HasOne(pb => pb.Banda)
                .WithMany()
                .HasForeignKey(pb => pb.IDdeBanda)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación de Evento con Cuenta
            modelBuilder.Entity<Evento>()
                .HasOne(e => e.Cuenta)
                .WithMany()
                .HasForeignKey(e => e.IDdeCuenta)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación de Banda con Cuenta (Musico)
            modelBuilder.Entity<Banda>()
                .HasOne(b => b.Musico)
                .WithMany()
                .HasForeignKey(b => b.IDdeMusico)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}