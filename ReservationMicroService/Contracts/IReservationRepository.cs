using ReservationMicroService.Models;
using ReservationMicroService.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReservationMicroService.Contracts
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllReservationsPaginationAsync(int pageIndex, int pageSize, ReservationModelForSearchDTO searchModel, bool trackChanges);
        Task<IEnumerable<Reservation>> GetAllUnboughtReservationsForMovie(DateTime dateTime);
        Task<int> GetReservationsCountAsync(ReservationModelForSearchDTO searchModel, bool trackChanges);
        Task<Reservation> GetReservationAsync(int ReservationId, bool trackChanges);
        Task<Reservation> GetReservationByDateTimeAndTel(DateTime dateTime, string telephone, bool trackChanges);
        Task<bool> IsPlacesFree(DateTime dateTime, IEnumerable<Place> places, int ReservationId);
        Task<Reservation> IsTelephoneHasAlreadyReservationForThisTime(string telephone, DateTime dateTime);
        void CreateReservation(Reservation Reservation);
        void DeleteReservation(Reservation Reservation);
        Task SaveAsync();
    }
}
