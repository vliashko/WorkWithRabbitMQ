using ReservationMicroService.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReservationMicroService.Contracts
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationForReadDTO>> GetReservationsPaginationAsync(int pageIndex, int pageSize, ReservationModelForSearchDTO searchModel);
        Task<int> GetReservationsCountAsync(ReservationModelForSearchDTO searchModel);
        Task<ReservationForReadDTO> GetReservationAsync(int id);
        Task<MessageDetailsForCreateDTO> CreateReservationAsync(ReservationForCreateDTO ReservationForCreateDTO);
        Task<MessageDetailsDTO> BuyReservationAsync(string telephone, DateTime dateTime);
        Task<MessageDetailsDTO> DeleteAllUnboughtReservationsForMovie(DateTime dateTime);
        Task<MessageDetailsDTO> DeleteReservationAsync(int id);
        Task<MessageDetailsDTO> UpdateReservationAsync(int id, ReservationForUpdateDTO ReservationForUpdateDTO);
    }
}
