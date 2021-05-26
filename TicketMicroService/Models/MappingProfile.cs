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
            CreateMap<TicketForUpdateDTO, Ticket>().ReverseMap();

            CreateMap<Place, PlaceForUpdateDTO>().ReverseMap();

            CreateMap<Place, PlaceShared>();
            CreateMap<Ticket, TicketShared>().ForMember(x => x.Places, c => c.MapFrom(x => x.Places));
        }
    }
}
