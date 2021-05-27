using Microsoft.EntityFrameworkCore;

namespace MovieMicroService.Models
{
    public class RepositoryDbContext : DbContext
    {
        public RepositoryDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Place> Places { get; set; }
    }
}
