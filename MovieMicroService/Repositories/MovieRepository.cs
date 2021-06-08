using MongoDB.Bson;
using MongoDB.Driver;
using MovieMicroService.Contracts;
using MovieMicroService.Models;
using MovieMicroService.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMicroService.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IMongoCollection<Movie> _movies;

        public MovieRepository(IMongoClient client)
        {
            var database = client.GetDatabase("movies");
            var collection = database.GetCollection<Movie>(nameof(Movie));
            _movies = collection;
        }


        public void CreateMovie(Movie movie)
        {
            _movies.InsertOneAsync(movie);
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesPaginationAsync(int pageIndex, int pageSize, MovieModelForSearchDTO searchModel)
        {
            var movies = await _movies.Find(movie => 
                (string.IsNullOrWhiteSpace(searchModel.Name) || movie.Name.Contains(searchModel.Name)) &&
                (searchModel.StartMovie == new DateTime() || searchModel.StartMovie == movie.StartMovie))
                .Skip((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
            return movies;
        }

        public async Task<Movie> GetMovieAsync(ObjectId movieId)
        {
            var movie = await _movies.Find(movie => movie.Id == movieId)
                .FirstOrDefaultAsync();
            return movie;
        }

        public async Task<Movie> GetMovieByDateTimeAsync(DateTime dateTime)
        {
            var movie = await _movies.Find(movie => movie.StartMovie == dateTime)
                .FirstOrDefaultAsync();
            return movie;
        }

        public async Task<long> GetMoviesCountAsync(MovieModelForSearchDTO searchModel)
        {
            var count = await _movies.Find(movie =>
                (string.IsNullOrWhiteSpace(searchModel.Name) || movie.Name.Contains(searchModel.Name)) &&
                (searchModel.StartMovie == new DateTime() || searchModel.StartMovie == movie.StartMovie))
                .CountDocumentsAsync();
            return count;
        }

        public async Task<bool> IsTimeValid(DateTime start, DateTime end)
        {
            if ((await _movies.Find(movie => (movie.StartMovie <= start && movie.EndMovie >= start) || (movie.StartMovie <= end && movie.EndMovie >= end) &&
                 (movie.StartMovie > start && movie.StartMovie < end) || (movie.EndMovie > start && movie.EndMovie < end))
                .ToListAsync()).Any())
                return false;
            return true;
        }
    }
}
