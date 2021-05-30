using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketMicroService.Contracts;
using TicketMicroService.Models;

namespace TicketMicroService.Repositories
{
    public class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
    {
        public ReservationRepository(RepositoryDbContext context) : base(context)
        {
        }

        public void AddReservation(Reservation reservation)
        {
            Create(reservation);
        }

        public void DeleteReservation(Reservation reservation)
        {
            Delete(reservation);
        }

        public async Task<bool> IsPlacesFree(DateTime dateTime, IEnumerable<Place> places)
        {
            var reservations = await FindByCondition(reservation =>
                reservation.DateTime == dateTime, false)
                .Include(ticket => ticket.Places)
                .ToListAsync();
            if (reservations.SelectMany(ticket =>
                            ticket.Places.SelectMany(place => places
                                .Where(aplace => aplace.Row == place.Row && aplace.Site == place.Site)
                                .Select(aplace => new { }))).Any())
                return false;
            return true;
        }

        public async Task SaveAsync() => await context.SaveChangesAsync();
    }
}
