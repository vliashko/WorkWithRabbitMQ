using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservationMicroService.Models;

namespace ReservationMicroService.Contracts
{
    public interface IMovieRepository
    {
        void AddMovie(Movie movie);
        Task<Movie> GetMovieAsync(DateTime dateTime, bool trackChanges);
        Task<bool> IsDateTimeCorrect(DateTime dateTime);
        Task<bool> IsPlacesCorrect(DateTime dateTime, IEnumerable<Place> places);
        Task SaveAsync();
    }
}
