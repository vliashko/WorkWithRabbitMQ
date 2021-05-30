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
    public class TicketService : ITicketService, IConsumer<MovieShared>, IConsumer<ReservationShared>, IConsumer<OrderToTicketShared>
    {
        private readonly ITicketRepository _repository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IBus _bus;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository repository, 
            IMovieRepository movieRepository, 
            IBus bus, IMapper mapper,
            IReservationRepository reservationRepository)
        {
            _repository = repository;
            _movieRepository = movieRepository;
            _bus = bus;
            _mapper = mapper;
            _reservationRepository = reservationRepository;
        }

        public async Task Consume(ConsumeContext<MovieShared> context)
        {
            var data = context.Message;
            if (data.Type == TypeOperation.Create)
            {
                _movieRepository.AddMovie(_mapper.Map<Movie>(data));
                await _movieRepository.SaveAsync();
            }
        }

        public async Task Consume(ConsumeContext<ReservationShared> context)
        {
            var data = context.Message;
            if(data.Type == TypeOperation.Create)
            {
                _reservationRepository.AddReservation(_mapper.Map<Reservation>(data));
            }
            else if(data.Type == TypeOperation.Delete)
            {
                _reservationRepository.DeleteReservation(_mapper.Map<Reservation>(data));
            }
            await _reservationRepository.SaveAsync();
        }

        public async Task Consume(ConsumeContext<OrderToTicketShared> context)
        {
            var data = context.Message;
            if(data.Type == TypeOperation.Create)
            {
                var ticket = await _repository.GetTicketByDateTimeAndEmail(data.DateTime, data.Email, true);
                ticket.PaymentCode = data.PaymentCode;
            }
            else if(data.Type == TypeOperation.Delete)
            {
                var ticket = await _repository.GetTicketByDateTimeAndEmail(data.DateTime, data.Email, false);
                _repository.DeleteTicket(ticket);
            }
            await _repository.SaveAsync();
        }

        public async Task<MessageDetailsForCreateDTO> CreateTicketAsync(TicketForCreateDTO TicketForCreateDTO)
        {
            var entity = _mapper.Map<Ticket>(TicketForCreateDTO);
            var isPlacesCorrect = await _movieRepository.IsPlacesCorrect(entity.DateTime, entity.Places);
            if (!isPlacesCorrect)
                return new MessageDetailsForCreateDTO { StatusCode = 406, Ticket = null };
            var isTimeCorrect = await _movieRepository.IsDateTimeCorrect(entity.DateTime);
            if (!isTimeCorrect)
                return new MessageDetailsForCreateDTO { StatusCode = 406, Ticket = null };
            var ReservationForThisTel =
                await _repository.IsEmailHasAlreadyTicketForThisTime(TicketForCreateDTO.Email, TicketForCreateDTO.DateTime);
            if (ReservationForThisTel != null)
                return new MessageDetailsForCreateDTO { StatusCode = 400, Ticket = _mapper.Map<TicketForReadDTO>(ReservationForThisTel) };
            var isSitesSame = IsSitesSame(entity.Places);
            if (isSitesSame)
                return new MessageDetailsForCreateDTO { StatusCode = 406, Ticket = null };
            var canBeCreated = await _repository.IsPlacesFree(entity.DateTime, entity.Places, 0);
            if (!canBeCreated)
                return new MessageDetailsForCreateDTO { StatusCode = 400, Ticket = null };
            canBeCreated = await _reservationRepository.IsPlacesFree(entity.DateTime, entity.Places);
            if (!canBeCreated)
                return new MessageDetailsForCreateDTO { StatusCode = 400, Ticket = null };
            _repository.CreateTicket(entity);
            await _repository.SaveAsync();
            var entityDto = _mapper.Map<TicketForReadDTO>(entity);

            Uri uri = new Uri("rabbitmq://localhost/TicketToMovieQueue?bind=true&queue=TicketToMovieQueue");
            var endPoint = await _bus.GetSendEndpoint(uri);
            var objBus = _mapper.Map<TicketShared>(entity);
            objBus.Type = TypeOperation.Create;
            await endPoint.Send(objBus);

            uri = new Uri("rabbitmq://localhost/TicketToReservationQueue?bind=true&queue=TicketToReservationQueue");
            endPoint = await _bus.GetSendEndpoint(uri);
            objBus = _mapper.Map<TicketShared>(entity);
            objBus.Type = TypeOperation.Create;
            await endPoint.Send(objBus);

            uri = new Uri("rabbitmq://localhost/TicketToOrderQueue?bind=true&queue=TicketToOrderQueue");
            endPoint = await _bus.GetSendEndpoint(uri);
            objBus = _mapper.Map<TicketShared>(entity);
            objBus.Type = TypeOperation.Create;
            await endPoint.Send(objBus);

            return new MessageDetailsForCreateDTO { StatusCode = 201, Ticket = entityDto };
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
                    return true;
            }
            return false;
        }
    }

}
