using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaDeOptimizacionAPI.Models;

namespace SistemaDeOptimizacionAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> Clientes { get; set; }
        public DbSet<Dueño> Dueños { get; set; }
        public DbSet<Perro> Perros { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Actividad> Actividades { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<ControlActividad> ControlActividades { get; set; }

       
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder
                    .UseSqlServer("Server=LAPTOP-PAPA\\SQLEXPRESS;Database=HM;Trusted_Connection=True;TrustServerCertificate=True;")
                    .LogTo(Console.WriteLine, LogLevel.Information);
            }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de claves foráneas con eliminación y actualización en cascada


            modelBuilder.Entity<ControlActividad>()
                .HasKey(ca => ca.ControlID); // Definir la clave primaria
            // Dueño - Perro (Eliminación y actualización en cascada)
            modelBuilder.Entity<Perro>()
                .HasOne(p => p.Dueño)
                .WithMany(d => d.Perros)
                .HasForeignKey(p => p.DueñoID)
                .OnDelete(DeleteBehavior.Cascade);

            // Perro - Reserva (Eliminación y actualización en cascada)
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Perro)
                .WithMany(p => p.Reservas)
                .HasForeignKey(r => r.PerroID)
                .OnDelete(DeleteBehavior.Cascade);

            // Servicio - Reserva (Eliminación y actualización en cascada)
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Servicio)
                .WithMany(s => s.Reservas)
                .HasForeignKey(r => r.ServicioID)
                .OnDelete(DeleteBehavior.Cascade);

            // Usuario - Reserva (Eliminación y actualización en cascada)
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Reservas)
                .HasForeignKey(r => r.UsuarioID)
                .OnDelete(DeleteBehavior.Cascade);

            // Promoción - Reserva (Eliminación en cascada)
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Promocion)
                .WithMany(p => p.Reservas)
                .HasForeignKey(r => r.PromocionID)
                .OnDelete(DeleteBehavior.Cascade);

            // ControlActividad - Reserva y Actividad (Eliminación y actualización en cascada)
            modelBuilder.Entity<ControlActividad>()
                .HasOne(ca => ca.Reserva)
                .WithMany(r => r.ControlActividades)
                .HasForeignKey(ca => ca.ReservaID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ControlActividad>()
                .HasOne(ca => ca.Actividad)
                .WithMany(a => a.ControlActividades)
                .HasForeignKey(ca => ca.ActividadID)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de tipos y precisión de decimales
            modelBuilder.Entity<Perro>()
                .Property(p => p.Peso)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Servicio>()
                .Property(s => s.Precio)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Actividad>()
                .Property(a => a.Precio)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Promocion>()
                .Property(p => p.DescuentoPorcentaje)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Reserva>()
                .Property(r => r.PrecioTotal)
                .HasColumnType("decimal(18,2)");

            // Índices adicionales
            modelBuilder.Entity<Perro>()
                .HasIndex(p => p.DueñoID)
                .HasDatabaseName("IDX_Perros_DueñoID");

            modelBuilder.Entity<Reserva>()
                .HasIndex(r => r.PerroID)
                .HasDatabaseName("IDX_Reservas_PerroID");

            modelBuilder.Entity<Reserva>()
                .HasIndex(r => r.UsuarioID)
                .HasDatabaseName("IDX_Reservas_UsuarioID");

            modelBuilder.Entity<Reserva>()
                .HasIndex(r => r.ServicioID)
                .HasDatabaseName("IDX_Reservas_ServicioID");

            modelBuilder.Entity<Reserva>()
                .HasIndex(r => r.PromocionID)
                .HasDatabaseName("IDX_Reservas_PromocionID");
        }
    }
}

