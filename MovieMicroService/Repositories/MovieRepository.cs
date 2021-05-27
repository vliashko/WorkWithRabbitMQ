using Microsoft.EntityFrameworkCore;
using MovieMicroService.Contracts;
using MovieMicroService.Models;
using MovieMicroService.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMicroService.Repositories
{
    public class MovieRepository : RepositoryBase<Movie>, IMovieRepository
    {
        public MovieRepository(RepositoryDbContext context): base(context)
        {   
        }

        public void CreateMovie(Movie movie)
        {
            Create(movie);
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesPaginationAsync(int pageIndex, int pageSize, MovieModelForSearchDTO searchModel, bool trackChanges)
        {
            return await FindByCondition(movie =>
                (string.IsNullOrWhiteSpace(searchModel.Name) || movie.Name.Contains(searchModel.Name)) &&
                (searchModel.StartMovie == new DateTime() || searchModel.StartMovie == movie.StartMovie), trackChanges)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Movie> GetMovieAsync(int movieId, bool trackChanges)
        {
            return await FindByCondition(movie => movie.Id == movieId, trackChanges)
                .Include(movie => movie.Places)
                .SingleOrDefaultAsync();
        }

        public async Task<Movie> GetMovieByDateTimeAsync(DateTime dateTime, bool trackChanges)
        {
            return await FindByCondition(movie => movie.StartMovie == dateTime, trackChanges)
                .Include(x => x.Places)
                .SingleOrDefaultAsync();
        }

        public async Task<int> GetMoviesCountAsync(MovieModelForSearchDTO searchModel, bool trackChanges)
        {
            return await FindByCondition(movie =>
                (string.IsNullOrWhiteSpace(searchModel.Name) || movie.Name.Contains(searchModel.Name)) &&
                (searchModel.StartMovie == new DateTime() || searchModel.StartMovie == movie.StartMovie), trackChanges)
                .CountAsync();
        }

        public async Task<bool> IsTimeValid(DateTime start, DateTime end)
        {
            var movies = await FindByCondition(movie => (movie.StartMovie.Date == start.Date || movie.StartMovie.Date == end.Date) &&
                (movie.EndMovie.Date == start.Date || movie.EndMovie.Date == end.Date), false)
                .ToListAsync();
            foreach (var movie in movies)
            {
                if ((movie.StartMovie <= start && movie.EndMovie >= start) || (movie.StartMovie <= end && movie.EndMovie >= end))
                    return false;
                if ((movie.StartMovie > start && movie.StartMovie < end) || (movie.EndMovie > start && movie.EndMovie < end))
                    return false;
            }
            return true;
        }

        public async Task SaveAsync() => await context.SaveChangesAsync();
    }
}
