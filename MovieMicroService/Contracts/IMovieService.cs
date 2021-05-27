using MovieMicroService.Models.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieMicroService.Contracts
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieForReadDTO>> GetMoviesPaginationAsync(int pageIndex, int pageSize, MovieModelForSearchDTO searchModel);
        Task<int> GetMoviesCountAsync(MovieModelForSearchDTO searchModel);
        Task<MovieForReadDTO> GetMovieAsync(int id);
        Task<MessageDetailsForCreateDTO> CreateMovieAsync(MovieForCreateDTO movieForCreateDTO);
        Task<MessageDetailsDTO> UpdateMovieAsync(int id, MovieForUpdateDTO movieForUpdateDTO);
    }
}
