using MongoDB.Bson;
using MovieMicroService.Models;
using MovieMicroService.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieMicroService.Contracts
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllMoviesPaginationAsync(int pageIndex, int pageSize, MovieModelForSearchDTO searchModel);
        Task<long> GetMoviesCountAsync(MovieModelForSearchDTO searchModel);
        Task<bool> IsTimeValid(DateTime start, DateTime end);
        Task<Movie> GetMovieAsync(ObjectId movieId);
        Task<Movie> GetMovieByDateTimeAsync(DateTime dateTime);
        void CreateMovie(Movie movie);
    }
}
