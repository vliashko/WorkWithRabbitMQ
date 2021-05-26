using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketMicroService.Models;
using TicketMicroService.Models.DataTransferObjects;

namespace TicketMicroService.Contracts
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllTicketsPaginationAsync(int pageIndex, int pageSize, TicketModelForSearchDTO searchModel, bool trackChanges);
        Task<int> GetTicketsCountAsync(TicketModelForSearchDTO searchModel, bool trackChanges);
        Task<Ticket> GetTicketAsync(int ticketId, bool trackChanges);
        Task<bool> IsPlacesFree(DateTime dateTime, IEnumerable<Place> places);
        Task<Ticket> IsTelephoneHasAlreadyTicketForThisTime(string telephone, DateTime dateTime);
        void CreateTicket(Ticket ticket);
        void DeleteTicket(Ticket ticket);
        Task SaveAsync();
    }
}
