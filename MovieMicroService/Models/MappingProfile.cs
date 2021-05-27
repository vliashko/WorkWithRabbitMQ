using AutoMapper;
using MovieMicroService.Models.DataTransferObjects;
using SharedModels;

namespace MovieMicroService.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, MovieForReadDTO>();
            CreateMap<MovieForCreateDTO, Movie>();
            CreateMap<MovieForUpdateDTO, Movie>().ReverseMap();

            CreateMap<Place, PlaceShared>();
            CreateMap<Movie, MovieShared>();
        }
    }
}
