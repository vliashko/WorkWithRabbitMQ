using AutoMapper;
using SharedModels;
using ReservationMicroService.Models.DataTransferObjects;

namespace ReservationMicroService.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Reservation, ReservationForReadDTO>();
            CreateMap<ReservationForCreateDTO, Reservation>();
            CreateMap<ReservationForUpdateDTO, Reservation>().ReverseMap();

            CreateMap<Movie, MovieShared>().ReverseMap();
            CreateMap<Place, PlaceShared>().ReverseMap();
            CreateMap<Reservation, ReservationShared>().ForMember(x => x.Places, c => c.MapFrom(x => x.Places)).ReverseMap();
        }
    }
}
