using AutoMapper;
using ReservationMicroService.Models.DataTransferObjects;
using SharedModels;

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
            CreateMap<Ticket, TicketShared>().ReverseMap();
            CreateMap<Reservation, ReservationShared>().ReverseMap();
        }
    }
}
