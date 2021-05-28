using System.Collections.Generic;
using System.Threading.Tasks;
using TicketMicroService.Models.DataTransferObjects;

namespace TicketMicroService.Contracts
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketForReadDTO>> GetTicketsPaginationAsync(int pageIndex, int pageSize, TicketModelForSearchDTO searchModel);
        Task<int> GetTicketsCountAsync(TicketModelForSearchDTO searchModel);
        Task<TicketForReadDTO> GetTicketAsync(int id);
        Task<MessageDetailsForCreateDTO> CreateTicketAsync(TicketForCreateDTO TicketForCreateDTO);
    }
}
