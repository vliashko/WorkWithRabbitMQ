using AutoMapper;
using SharedModels;
using TicketMicroService.Models.DataTransferObjects;

namespace TicketMicroService.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Ticket, TicketForReadDTO>();
            CreateMap<TicketForCreateDTO, Ticket>();

            CreateMap<Movie, MovieShared>().ReverseMap();
            CreateMap<Place, PlaceShared>().ReverseMap();
            CreateMap<Ticket, TicketShared>().ReverseMap();
            CreateMap<Reservation, ReservationShared>().ReverseMap();
        }
    }
}
