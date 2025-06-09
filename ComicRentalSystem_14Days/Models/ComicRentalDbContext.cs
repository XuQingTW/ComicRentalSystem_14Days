using Microsoft.EntityFrameworkCore;
using ComicRentalSystem_14Days.Helpers; // For DatabaseConfig

// Assuming entities will be in this namespace or accessible
// using ComicRentalSystem_14Days.Models;

namespace ComicRentalSystem_14Days.Models // Or ComicRentalSystem_14Days.Data
{
    public class ComicRentalDbContext : DbContext
    {
        public DbSet<Comic> Comics { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<User> Users { get; set; }

        public ComicRentalDbContext()
        {
        }

        // Constructor for passing options, useful for testing or specific configurations
        public ComicRentalDbContext(DbContextOptions<ComicRentalDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(DatabaseConfig.GetConnectionString());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example of fluent API configuration if needed later (e.g., for composite keys, indexes)
            // modelBuilder.Entity<Comic>().ToTable("Comics");
            // modelBuilder.Entity<Member>().ToTable("Members");
            // modelBuilder.Entity<User>().ToTable("Users");
        }
    }
}
