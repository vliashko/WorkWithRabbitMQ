using Microsoft.EntityFrameworkCore;

namespace TicketMicroService.Models
{
    public class RepositoryDbContext : DbContext
    {
        public RepositoryDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Place> Places { get; set; }
    }
}
