using Microsoft.EntityFrameworkCore;
using ReservationMicroService.Contracts;
using ReservationMicroService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationMicroService.Repositories
{
    public class TicketRepository : RepositoryBase<Ticket>, ITicketRepository
    {
        public TicketRepository(RepositoryDbContext context) : base(context)
        {
        }

        public void AddTicket(Ticket ticket)
        {
            Create(ticket);
        }

        public void DeleteTicket(Ticket ticket)
        {
            Delete(ticket);
        }

        public async Task<bool> IsPlacesFree(DateTime dateTime, IEnumerable<Place> places)
        {
            var tickets = await FindByCondition(ticket =>
                ticket.DateTime == dateTime, false)
                .Include(ticket => ticket.Places)
                .ToListAsync();
            if (tickets.SelectMany(ticket =>
                            ticket.Places.SelectMany(place => places
                                .Where(aplace => aplace.Row == place.Row && aplace.Site == place.Site)
                                .Select(aplace => new { }))).Any())
                return false;
            return true;
        }

        public async Task SaveAsync() => await context.SaveChangesAsync();
    }
}
