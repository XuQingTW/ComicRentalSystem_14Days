using Microsoft.EntityFrameworkCore;
using ComicRentalSystem_14Days.Helpers; 


namespace ComicRentalSystem_14Days.Models 
{
    public class ComicRentalDbContext : DbContext
    {
        public DbSet<Comic> Comics { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<User> Users { get; set; }

        public ComicRentalDbContext()
        {
        }

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

        }
    }
}
