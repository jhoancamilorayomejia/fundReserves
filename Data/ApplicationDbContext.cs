using Microsoft.EntityFrameworkCore;
using FoundReserves.Models;

namespace FoundReserves.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Sede> Sedes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ── TABLA USERS ──
            builder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.iduser);
                entity.Property(e => e.iduser).ValueGeneratedOnAdd();
                entity.Property(e => e.cedula).IsRequired().HasMaxLength(20);
                entity.Property(e => e.name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.lastname).IsRequired().HasMaxLength(100);
                entity.Property(e => e.phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.password).IsRequired().HasMaxLength(500);
                entity.Property(e => e.rol).IsRequired().HasMaxLength(50);
                entity.Property(e => e.createdAt).HasDefaultValueSql("GETDATE()");

                // Campos nuevos — opcionales para no romper registros anteriores
                entity.Property(e => e.fechaNacimiento).IsRequired(false);
                entity.Property(e => e.departamento).IsRequired(false).HasMaxLength(100);
                entity.Property(e => e.municipio).IsRequired(false).HasMaxLength(100);
                entity.Property(e => e.barrio).IsRequired(false).HasMaxLength(100);
                entity.Property(e => e.direccion).IsRequired(false).HasMaxLength(255);
                entity.Property(e => e.preguntaSecreta).IsRequired(false).HasMaxLength(255);
                entity.Property(e => e.respuestaSecreta).IsRequired(false).HasMaxLength(255);
                entity.Property(e => e.autorizaCorreo).IsRequired(false);
                entity.Property(e => e.autorizaCelular).IsRequired(false);
            });

            // ── TABLA SEDES ──
            builder.Entity<Sede>(entity =>
            {
                entity.ToTable("sedes");
                entity.HasKey(e => e.idSede);
                entity.Property(e => e.idSede).ValueGeneratedOnAdd();
                entity.Property(e => e.name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.city).IsRequired().HasMaxLength(100);
                entity.Property(e => e.region).IsRequired().HasMaxLength(100);
                entity.Property(e => e.description).IsRequired(false);
                entity.Property(e => e.maximumCapacity).IsRequired();
                entity.Property(e => e.type).IsRequired().HasMaxLength(100);
                entity.Property(e => e.priceLaundry).HasColumnType("decimal(10,2)");
            });
        }
    }
}