using MovieMicroService.Models;
using MovieMicroService.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieMicroService.Contracts
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllMoviesPaginationAsync(int pageIndex, int pageSize, MovieModelForSearchDTO searchModel, bool trackChanges);
        Task<int> GetMoviesCountAsync(MovieModelForSearchDTO searchModel, bool trackChanges);
        Task<bool> IsTimeValid(DateTime start, DateTime end);
        Task<Movie> GetMovieAsync(int movieId, bool trackChanges);
        Task<Movie> GetMovieByDateTimeAsync(DateTime dateTime, bool trackChanges);
        void CreateMovie(Movie movie);
        Task SaveAsync();
    }
}
