using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReservationMicroService.Contracts;
using ReservationMicroService.Models;

namespace ReservationMicroService.Repositories
{
    public class MovieRepository : RepositoryBase<Movie>, IMovieRepository
    {
        public MovieRepository(RepositoryDbContext context) : base(context)
        {
        }

        public void AddMovie(Movie movie)
        {
            Create(movie);
        }

        public async Task<Movie> GetMovieAsync(DateTime dateTime, bool trackChanges)
        {
            var movie = await FindByCondition(movie => movie.StartMovie == dateTime, false).SingleOrDefaultAsync();
            return movie;
        }

        public async Task<bool> IsDateTimeCorrect(DateTime dateTime)
        {
            var movie = await FindByCondition(movie => movie.StartMovie == dateTime, false).SingleOrDefaultAsync();
            if (movie == null)
                return false;
            return true;
        }

        public async Task<bool> IsPlacesCorrect(DateTime dateTime, IEnumerable<Place> places)
        {
            var movie = await FindByCondition(movie => movie.StartMovie == dateTime, false).SingleOrDefaultAsync();
            if (movie == null)
                return false;

            foreach (var place in places)
            {
                if (place.Row > movie.CountRows || place.Row < 1 || place.Site > movie.CountSites || place.Site < 1)
                    return false;
            }
            return true;
        }

        public async Task SaveAsync() => await context.SaveChangesAsync();
    }
}
