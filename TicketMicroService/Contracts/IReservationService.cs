using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservationMicroService.Models.DataTransferObjects;

namespace ReservationMicroService.Contracts
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationForReadDTO>> GetReservationsPaginationAsync(int pageIndex, int pageSize, ReservationModelForSearchDTO searchModel);
        Task<int> GetReservationsCountAsync(ReservationModelForSearchDTO searchModel);
        Task<ReservationForReadDTO> GetReservationAsync(int id);
        Task<MessageDetailsForCreateDTO> CreateReservationAsync(ReservationForCreateDTO ReservationForCreateDTO);
        Task<MessageDetailsDTO> DeleteAllUnboughtReservationsForMovie(DateTime dateTime);
        Task<MessageDetailsDTO> DeleteReservationAsync(int id);
        Task<MessageDetailsDTO> UpdateReservationAsync(int id, ReservationForUpdateDTO ReservationForUpdateDTO);
    }
}
