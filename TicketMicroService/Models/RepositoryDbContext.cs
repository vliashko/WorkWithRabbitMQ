using Microsoft.EntityFrameworkCore;

namespace TicketMicroService.Models
{
    public class RepositoryDbContext : DbContext
    {
        public RepositoryDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
