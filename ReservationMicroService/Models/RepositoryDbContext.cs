using Microsoft.EntityFrameworkCore;

namespace ReservationMicroService.Models
{
    public class RepositoryDbContext : DbContext
    {
        public RepositoryDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Movie> Movies { get; set; }
    }
}
