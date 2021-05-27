using Microsoft.EntityFrameworkCore;
using ReservationMicroService.Contracts;
using ReservationMicroService.Models;
using ReservationMicroService.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationMicroService.Repositories
{
    public class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
    {
        public ReservationRepository(RepositoryDbContext context) : base(context)
        {
        }

        public void CreateReservation(Reservation Reservation)
        {
            Create(Reservation);
        }

        public void DeleteReservation(Reservation Reservation)
        {
            Delete(Reservation);
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsPaginationAsync(int pageIndex, int pageSize,
            ReservationModelForSearchDTO searchModel, bool trackChanges)
        {
            return await FindByCondition(Reservation =>
                (string.IsNullOrWhiteSpace(searchModel.Telephone) || Reservation.Telephone.Contains(searchModel.Telephone)) &&
                (searchModel.DateTime == new DateTime() || searchModel.DateTime == Reservation.DateTime), trackChanges)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(Reservation => Reservation.Places)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetAllUnboughtReservationsForMovie(DateTime dateTime)
        {
            return await FindByCondition(Reservation => Reservation.DateTime == dateTime && Reservation.IsFooled == false, false)
                .ToListAsync();
        }

        public async Task<Reservation> GetReservationAsync(int ReservationId, bool trackChanges)
        {
            return await FindByCondition(Reservation => Reservation.Id == ReservationId, trackChanges)
                .Include(Reservation => Reservation.Places)
                .SingleOrDefaultAsync();
        }

        public async Task<Reservation> GetReservationByDateTimeAndTel(DateTime dateTime, string telephone, bool trackChanges)
        {
            return await FindByCondition(Reservation => Reservation.DateTime == dateTime && Reservation.Telephone == telephone, trackChanges)
                .Include(Reservation => Reservation.Places)
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetReservationsCountAsync(ReservationModelForSearchDTO searchModel, bool trackChanges)
        {
            return await FindByCondition(Reservation =>
                (string.IsNullOrWhiteSpace(searchModel.Telephone) || Reservation.Telephone.Contains(searchModel.Telephone)) &&
                (searchModel.DateTime == new DateTime() || searchModel.DateTime == Reservation.DateTime), trackChanges)
                .CountAsync();
        }

        public async Task<bool> IsPlacesFree(DateTime dateTime, IEnumerable<Place> places, int ReservationId)
        {
            var Reservations = await FindByCondition(Reservation =>
                Reservation.DateTime == dateTime && Reservation.Id != ReservationId, false)
                .Include(Reservation => Reservation.Places)
                .ToListAsync();
            if (Reservations.SelectMany(Reservation =>
                            Reservation.Places.SelectMany(place => places
                                .Where(aplace => aplace.Row == place.Row && aplace.Site == place.Site)
                                .Select(aplace => new { }))).Any())
                return false;

            return true;
        }

        public async Task<Reservation> IsTelephoneHasAlreadyReservationForThisTime(string telephone, DateTime dateTime)
        {
            var res = await FindByCondition(t => t.Telephone == telephone && t.DateTime == dateTime, false)
                .Include(x => x.Places)
                .SingleOrDefaultAsync();
            if (res != null)
                return res;
            return null;
        }

        public async Task SaveAsync() => await context.SaveChangesAsync();
    }
}
