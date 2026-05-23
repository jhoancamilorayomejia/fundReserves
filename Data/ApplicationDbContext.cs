using Microsoft.EntityFrameworkCore;
using FoundReserves.Models;

namespace FoundReserves.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options
        ) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Sede> Sedes { get; set; }

        public DbSet<Accommodation> Accommodations { get; set; }

        public DbSet<Season> Seasons { get; set; }

        public DbSet<Rate> Rates { get; set; }
         public DbSet<Reserve> Reserves { get; set; }
         public DbSet<DetalleReserve> DetalleReserves { get; set; }

        protected override void OnModelCreating(
            ModelBuilder builder
        )
        {
            base.OnModelCreating(builder);

            // ─────────────────────────────
            // TABLA USERS
            // ─────────────────────────────
            builder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasKey(e => e.iduser);

                entity.Property(e => e.iduser)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.cedula)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.lastname)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.phone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.password)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.rol)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.createdAt)
                    .HasDefaultValueSql("GETDATE()");

                // Campos adicionales
                entity.Property(e => e.fechaNacimiento)
                    .IsRequired(false);

                entity.Property(e => e.departamento)
                    .IsRequired(false)
                    .HasMaxLength(100);

                entity.Property(e => e.municipio)
                    .IsRequired(false)
                    .HasMaxLength(100);

                entity.Property(e => e.barrio)
                    .IsRequired(false)
                    .HasMaxLength(100);

                entity.Property(e => e.direccion)
                    .IsRequired(false)
                    .HasMaxLength(255);

                entity.Property(e => e.preguntaSecreta)
                    .IsRequired(false)
                    .HasMaxLength(255);

                entity.Property(e => e.respuestaSecreta)
                    .IsRequired(false)
                    .HasMaxLength(255);

                entity.Property(e => e.autorizaCorreo)
                    .IsRequired(false);

                entity.Property(e => e.autorizaCelular)
                    .IsRequired(false);

                entity.Property(e => e.ResetToken)
                    .IsRequired(false)
                    .HasMaxLength(200);

                entity.Property(e => e.ResetTokenExpiration)
                    .IsRequired(false);
            });

            // ─────────────────────────────
            // TABLA SEDES
            // ─────────────────────────────
            builder.Entity<Sede>(entity =>
            {
                entity.ToTable("sedes");

                entity.HasKey(e => e.idSede);

                entity.Property(e => e.idSede)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.city)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.region)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.description)
                    .IsRequired(false);

                entity.Property(e => e.maximumCapacity)
                    .IsRequired();

                entity.Property(e => e.type)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.priceLaundry)
                    .HasColumnType("decimal(10,2)");
            });

            // ─────────────────────────────
            // TABLA ACCOMMODATION
            // ─────────────────────────────
            builder.Entity<Accommodation>(entity =>
            {
                entity.ToTable("accommodation");

                entity.HasKey(e => e.idAccommodation);

                entity.Property(e => e.idAccommodation)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.idsede)
                    .IsRequired();

                entity.Property(e => e.name)
                    .IsRequired(false)
                    .HasMaxLength(150);

                entity.Property(e => e.number)
                    .IsRequired(false)
                    .HasMaxLength(50);

                entity.Property(e => e.maximumPerson)
                    .IsRequired();

                entity.Property(e => e.description)
                    .IsRequired(false);

                entity.Property(e => e.state)
                    .IsRequired(false)
                    .HasMaxLength(50);

                entity.HasOne(e => e.Sede)
                    .WithMany()
                    .HasForeignKey(e => e.idsede)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ─────────────────────────────
            // TABLA SEASONS
            // ─────────────────────────────
            builder.Entity<Season>(entity =>
            {
                entity.ToTable("seasons");

                entity.HasKey(e => e.idSeason);

                entity.Property(e => e.idSeason)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name)
                .IsRequired()
                .HasMaxLength(100);

                entity.Property(e => e.type)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // ─────────────────────────────
            // TABLA RATES
            // ─────────────────────────────
            builder.Entity<Rate>(entity =>
            {
                entity.ToTable("rates");

                entity.HasKey(e => e.idPrice);

                entity.Property(e => e.idPrice)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.idAccommodation)
                    .IsRequired();

                entity.Property(e => e.idSeason)
                    .IsRequired();

                entity.Property(e => e.minimumPerson)
                    .IsRequired();

                entity.Property(e => e.maximumPerson)
                    .IsRequired();

                entity.Property(e => e.priceNight)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.pricePersonAdditional)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.HasOne<Accommodation>()
                    .WithMany()
                    .HasForeignKey(e => e.idAccommodation)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Season>()
                    .WithMany()
                    .HasForeignKey(e => e.idSeason)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ─────────────────────────────
            // TABLA RESERVES
            // ─────────────────────────────
            builder.Entity<Reserve>(entity =>
            {
                entity.ToTable("reserves");

                entity.HasKey(e => e.IdReserve);

                entity.Property(e => e.IdReserve)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.IdUser)
                    .IsRequired();

                entity.Property(e => e.IdAccommodation)
                    .IsRequired();

                entity.Property(e => e.DateStart)
                    .IsRequired();

                entity.Property(e => e.DateFinish)
                    .IsRequired();

                entity.Property(e => e.NumberPerson)
                    .IsRequired();

                entity.Property(e => e.TotalCal)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.DateCreation)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.ServiceLaundry)
                    .IsRequired();

                // FK → USERS
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.IdUser)
                    .OnDelete(DeleteBehavior.Cascade);

                // FK → ACCOMMODATION
                entity.HasOne<Accommodation>()
                    .WithMany()
                    .HasForeignKey(e => e.IdAccommodation)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ── DetalleReserves ──
builder.Entity<DetalleReserve>(entity =>
{
    entity.ToTable("detalleReserve");

    entity.HasKey(e => e.IdDetalleReserve);

    entity.Property(e => e.IdDetalleReserve)
        .ValueGeneratedOnAdd();

    entity.Property(e => e.IdUser)
        .IsRequired();

    entity.Property(e => e.IdSede)
        .IsRequired();

    entity.Property(e => e.DateStart)
        .IsRequired();

    entity.Property(e => e.DateFinish)
        .IsRequired();

    entity.Property(e => e.NumberPerson)
        .IsRequired();

    entity.Property(e => e.NumberRooms)
        .IsRequired();

    entity.Property(e => e.TotalCal)
        .HasColumnType("decimal(18,2)")
        .IsRequired();

    entity.Property(e => e.PaymentProof)
        .IsRequired(false)
        .HasMaxLength(255);

        entity.Property(e => e.CreatedAt)
    .HasDefaultValueSql("GETDATE()");

    // FK → users
    entity.HasOne<User>()
        .WithMany()
        .HasForeignKey(e => e.IdUser)
        .OnDelete(DeleteBehavior.Cascade);

    // FK → sedes
    entity.HasOne<Sede>()
        .WithMany()
        .HasForeignKey(e => e.IdSede)
        .OnDelete(DeleteBehavior.Cascade);

        
});
        }
    }
}