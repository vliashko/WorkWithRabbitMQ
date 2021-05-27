using AutoMapper;
using MassTransit;
using ReservationMicroService.Contracts;
using ReservationMicroService.Models;
using ReservationMicroService.Models.DataTransferObjects;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReservationMicroService.Services
{
    public class ReservationService : IReservationService, IConsumer<MovieShared>, IConsumer<OrderToReservation>
    {
        private readonly IReservationRepository _repository;
        private readonly IMovieRepository _movieRepository;
        private readonly IBus _bus;
        private readonly IMapper _mapper;

        public ReservationService(IReservationRepository repository, IMovieRepository movieRepository, IBus bus, IMapper mapper)
        {
            _repository = repository;
            _movieRepository = movieRepository;
            _bus = bus;
            _mapper = mapper;
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

        public async Task Consume(ConsumeContext<OrderToReservation> context)
        {
            var data = context.Message;
            var Reservation = await _repository.GetReservationByDateTimeAndTel(data.DateTime, data.Telephone, true);
            if (data.Type == TypeOperation.Create)
                Reservation.IsFooled = true;
            else if (data.Type == TypeOperation.Delete)
                Reservation.IsFooled = false;
            await _repository.SaveAsync();
        }

        public async Task<MessageDetailsForCreateDTO> CreateReservationAsync(ReservationForCreateDTO ReservationForCreateDTO)
        {
            var entity = _mapper.Map<Reservation>(ReservationForCreateDTO);
            var isPlacesCorrect = await _movieRepository.IsPlacesCorrect(entity.DateTime, entity.Places);
            if (!isPlacesCorrect)
                return new MessageDetailsForCreateDTO { StatusCode = 406, Reservation = null };
            var isTimeCorrect = await _movieRepository.IsDateTimeCorrect(entity.DateTime);
            if (!isTimeCorrect)
                return new MessageDetailsForCreateDTO { StatusCode = 406, Reservation = null };
            var ReservationForThisTel =
                await _repository.IsTelephoneHasAlreadyReservationForThisTime(ReservationForCreateDTO.Telephone, ReservationForCreateDTO.DateTime);
            if (ReservationForThisTel != null)
                return new MessageDetailsForCreateDTO { StatusCode = 400, Reservation = _mapper.Map<ReservationForReadDTO>(ReservationForThisTel) };
            var isSitesSame = IsSitesSame(entity.Places);
            if (isSitesSame)
                return new MessageDetailsForCreateDTO { StatusCode = 406, Reservation = null };
            var canBeCreated = await _repository.IsPlacesFree(entity.DateTime, entity.Places, 0);
            if (!canBeCreated)
                return new MessageDetailsForCreateDTO { StatusCode = 400, Reservation = null };
            _repository.CreateReservation(entity);
            await _repository.SaveAsync();
            var entityDto = _mapper.Map<ReservationForReadDTO>(entity);

            Uri uri = new Uri("rabbitmq://localhost/ReservationQueue?bind=true&queue=ReservationQueue");
            var endPoint = await _bus.GetSendEndpoint(uri);
            var objBus = _mapper.Map<ReservationShared>(entity);
            objBus.Type = TypeOperation.Create;
            await endPoint.Send(objBus);

            return new MessageDetailsForCreateDTO { StatusCode = 201, Reservation = entityDto };
        }

        public async Task<MessageDetailsDTO> DeleteAllUnboughtReservationsForMovie(DateTime dateTime)
        {
            var Reservations = await _repository.GetAllUnboughtReservationsForMovie(dateTime);
            foreach (var Reservation in Reservations)
            {
                Uri uri = new Uri("rabbitmq://localhost/ReservationQueue?bind=true&queue=ReservationQueue");
                var endPoint = await _bus.GetSendEndpoint(uri);
                var objBus = _mapper.Map<ReservationShared>(Reservation);
                objBus.Type = TypeOperation.Delete;
                await endPoint.Send(objBus);
                _repository.DeleteReservation(Reservation);
            }
            await _repository.SaveAsync();
            return new MessageDetailsDTO { StatusCode = 204 };
        }

        public async Task<MessageDetailsDTO> DeleteReservationAsync(int id)
        {
            var Reservation = await _repository.GetReservationAsync(id, false);
            if (Reservation == null)
                return new MessageDetailsDTO { StatusCode = 404, Message = $"Reservation with id: {id} doesn't exist in the database" };

            Uri uri = new Uri("rabbitmq://localhost/ReservationQueue?bind=true&queue=ReservationQueue");
            var endPoint = await _bus.GetSendEndpoint(uri);
            var objBus = _mapper.Map<ReservationShared>(Reservation);
            objBus.Type = TypeOperation.Delete;
            await endPoint.Send(objBus);

            _repository.DeleteReservation(Reservation);
            await _repository.SaveAsync();
            return new MessageDetailsDTO { StatusCode = 204 };
        }

        public async Task<ReservationForReadDTO> GetReservationAsync(int id)
        {
            var Reservation = await _repository.GetReservationAsync(id, false);
            if (Reservation == null)
                return null;
            return _mapper.Map<ReservationForReadDTO>(Reservation);
        }

        public async Task<int> GetReservationsCountAsync(ReservationModelForSearchDTO searchModel)
        {
            var count = await _repository.GetReservationsCountAsync(searchModel, false);
            return count;
        }

        public async Task<IEnumerable<ReservationForReadDTO>> GetReservationsPaginationAsync(int pageIndex, int pageSize, ReservationModelForSearchDTO searchModel)
        {
            var Reservations = await _repository.GetAllReservationsPaginationAsync(pageIndex, pageSize, searchModel, false);
            return _mapper.Map<IEnumerable<ReservationForReadDTO>>(Reservations);
        }

        public async Task<MessageDetailsDTO> UpdateReservationAsync(int id, ReservationForUpdateDTO ReservationForUpdateDTO)
        {
            var Reservation = await _repository.GetReservationAsync(id, true);
            if (Reservation == null)
                return new MessageDetailsDTO { StatusCode = 404, Message = $"Reservation with id: {id} doesn't exist in the database" };
            var isSitesSame = IsSitesSame(_mapper.Map<IEnumerable<Place>>(ReservationForUpdateDTO.Places));
            if (isSitesSame)
                return new MessageDetailsDTO { StatusCode = 400, Message = "Identical places selected." };
            var isPlacesFree = await _repository.IsPlacesFree(Reservation.DateTime, _mapper.Map<IEnumerable<Place>>(ReservationForUpdateDTO.Places), Reservation.Id);
            if (!isPlacesFree)
                return new MessageDetailsDTO { StatusCode = 400, Message = $"Reservation cannot be change. These seats have already been purchased / booked." };
            _mapper.Map(ReservationForUpdateDTO, Reservation);
            await _repository.SaveAsync();

            Uri uri = new Uri("rabbitmq://localhost/ReservationQueue?bind=true&queue=ReservationQueue");
            var endPoint = await _bus.GetSendEndpoint(uri);
            var objBus = _mapper.Map<ReservationShared>(Reservation);
            objBus.Type = TypeOperation.Edit;
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
                    return true;
            }
            return false;
        }
    }
}
