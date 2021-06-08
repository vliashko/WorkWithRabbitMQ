using MongoDB.Bson;
using MovieMicroService.Models.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieMicroService.Contracts
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieForReadDTO>> GetMoviesPaginationAsync(int pageIndex, int pageSize, MovieModelForSearchDTO searchModel);
        Task<long> GetMoviesCountAsync(MovieModelForSearchDTO searchModel);
        Task<MovieForReadDTO> GetMovieAsync(ObjectId id);
        Task<MessageDetailsForCreateDTO> CreateMovieAsync(MovieForCreateDTO movieForCreateDTO);
        Task<MessageDetailsDTO> UpdateMovieAsync(ObjectId id, MovieForUpdateDTO movieForUpdateDTO);
    }
}
