using AutoMapper;
using OrderMicroService.Models.DataTransferObjects;
using SharedModels;

namespace OrderMicroService.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderForReadDTO>();
            CreateMap<Order, OrderToReservationShared>();
            CreateMap<Order, OrderToTicketShared>();
        }
    }
}
