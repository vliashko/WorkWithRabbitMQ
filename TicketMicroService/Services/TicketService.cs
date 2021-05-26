using AutoMapper;
using MassTransit;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketMicroService.Contracts;
using TicketMicroService.Models;
using TicketMicroService.Models.DataTransferObjects;

namespace TicketMicroService.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repository;
        private readonly IBus _bus;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository repository, IBus bus, IMapper mapper)
        {
            _repository = repository;
            _bus = bus;
            _mapper = mapper;
        }

        public async Task<MessageDetailsForCreateDTO> CreateTicketAsync(TicketForCreateDTO ticketForCreateDTO)
        {
            var entity = _mapper.Map<Ticket>(ticketForCreateDTO);
            var ticketForThisTel =
                await _repository.IsTelephoneHasAlreadyTicketForThisTime(ticketForCreateDTO.Telephone, ticketForCreateDTO.DateTime);
            if (ticketForThisTel != null)
                return new MessageDetailsForCreateDTO { StatusCode = 400, Ticket = _mapper.Map<TicketForReadDTO>(ticketForThisTel) };
            var isSitesSame = IsSitesSame(entity.Places);
            if (isSitesSame)
                return new MessageDetailsForCreateDTO { StatusCode = 406, Ticket = null };
            var canBeCreated = await _repository.IsPlacesFree(entity.DateTime, entity.Places);
            if (!canBeCreated)
                return new MessageDetailsForCreateDTO { StatusCode = 400, Ticket = null };
            _repository.CreateTicket(entity);
            await _repository.SaveAsync();
            var entityDto = _mapper.Map<TicketForReadDTO>(entity);

            Uri uri = new Uri("rabbitmq://localhost/ticketQueueCreate?bind=true&queue=ticketQueueCreate");
            var endPoint = await _bus.GetSendEndpoint(uri);
            var objBus = _mapper.Map<TicketShared>(entity);
            await endPoint.Send(objBus);

            return new MessageDetailsForCreateDTO { StatusCode = 201, Ticket = entityDto };
        }

        public async Task<MessageDetailsDTO> DeleteTicketAsync(int id)
        {
            var ticket = await _repository.GetTicketAsync(id, false);
            if(ticket == null)
                return new MessageDetailsDTO { StatusCode = 404, Message = $"Ticket with id: {id} doesn't exist in the database" };

            Uri uri = new Uri("rabbitmq://localhost/ticketQueueCreate?bind=true&queue=ticketQueueDelete");
            var endPoint = await _bus.GetSendEndpoint(uri);
            var objBus = _mapper.Map<TicketShared>(ticket);
            await endPoint.Send(objBus);

            _repository.DeleteTicket(ticket);
            await _repository.SaveAsync();
            return new MessageDetailsDTO { StatusCode = 204 };
        }

        public async Task<TicketForReadDTO> GetTicketAsync(int id)
        {
            var ticket = await _repository.GetTicketAsync(id, false);
            if (ticket == null)
                return null;
            return _mapper.Map<TicketForReadDTO>(ticket);
        }

        public async Task<int> GetTicketsCountAsync(TicketModelForSearchDTO searchModel)
        {
            var count = await _repository.GetTicketsCountAsync(searchModel, false);
            return count;
        }

        public async Task<IEnumerable<TicketForReadDTO>> GetTicketsPaginationAsync(int pageIndex, int pageSize, TicketModelForSearchDTO searchModel)
        {
            var tickets = await _repository.GetAllTicketsPaginationAsync(pageIndex, pageSize, searchModel, false);
            return _mapper.Map<IEnumerable<TicketForReadDTO>>(tickets);
        }

        public async Task<MessageDetailsDTO> UpdateTicketAsync(int id, TicketForUpdateDTO ticketForUpdateDTO)
        {
            var ticket = await _repository.GetTicketAsync(id, true);
            if (ticket == null)
                return new MessageDetailsDTO { StatusCode = 404, Message = $"Ticket with id: {id} doesn't exist in the database" };
            var isPlacesFree = await _repository.IsPlacesFree(ticketForUpdateDTO.DateTime, ticketForUpdateDTO.Places);
            if (!isPlacesFree)
                return new MessageDetailsDTO { StatusCode = 400, Message = $"Ticket cannot be change. These seats have already been purchased / booked."};
            _mapper.Map(ticketForUpdateDTO, ticket);
            await _repository.SaveAsync();

            Uri uri = new Uri("rabbitmq://localhost/ticketQueueCreate?bind=true&queue=ticketQueueEdit");
            var endPoint = await _bus.GetSendEndpoint(uri);
            var objBus = _mapper.Map<TicketShared>(ticket);
            await endPoint.Send(objBus);

            return new MessageDetailsDTO { StatusCode = 204 };
        }

        private bool IsSitesSame(IEnumerable<Place> places)
        {
            foreach (var item1 in places)
            {
                int count = 0;
                foreach (var item2 in places)
                {
                    if (item1.Row == item2.Row && item1.Site == item2.Site)
                        count++;
                }
                if (count > 1)
                    return false;
            }
            return true;
        }
    }
}
