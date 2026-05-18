using Microsoft.EntityFrameworkCore;
using FoundReserves.Models;

namespace FoundReserves.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).ValueGeneratedOnAdd();
                entity.Property(e => e.email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.password).IsRequired().HasMaxLength(500);
                entity.Property(e => e.rol).IsRequired().HasMaxLength(50);
                entity.Property(e => e.createdAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
