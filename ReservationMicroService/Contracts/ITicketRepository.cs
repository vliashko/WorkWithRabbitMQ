using ReservationMicroService.Models;
using System;
using System.Collections.Generic; 
using System.Threading.Tasks;

namespace ReservationMicroService.Contracts
{
    public interface ITicketRepository
    {
        void AddTicket(Ticket ticket);
        void DeleteTicket(Ticket ticket);
        Task<bool> IsPlacesFree(DateTime dateTime, IEnumerable<Place> places);
        Task SaveAsync();
    }
}
