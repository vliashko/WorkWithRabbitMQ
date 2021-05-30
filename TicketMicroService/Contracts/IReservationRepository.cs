using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketMicroService.Models;

namespace TicketMicroService.Contracts
{
    public interface IReservationRepository
    {
        void AddReservation(Reservation reservation);
        void DeleteReservation(Reservation reservation);
        Task<bool> IsPlacesFree(DateTime dateTime, IEnumerable<Place> places);
        Task SaveAsync();
    }
}
