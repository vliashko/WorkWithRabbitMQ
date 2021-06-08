using AutoMapper;
using MassTransit;
using MongoDB.Bson;
using MovieMicroService.Contracts;
using MovieMicroService.Models;
using MovieMicroService.Models.DataTransferObjects;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieMicroService.Services
{
    public class MovieService : IMovieService, IConsumer<ReservationShared>, IConsumer<TicketShared>
    {
        private readonly IMovieRepository _repository;
        private readonly IBus _bus;
        private readonly IMapper _mapper;

        public MovieService(IMovieRepository repository, IBus bus, IMapper mapper)
        {
            _repository = repository;
            _bus = bus;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<ReservationShared> context)
        {
            var data = context.Message;
            if(data.Type == TypeOperation.Create)
            {
                var movie = await _repository.GetMovieByDateTimeAsync(data.DateTime);
                foreach (var place in movie.Places)
                {
                    foreach (var item in data.Places)
                    {
                        if (item.Row == place.Row && item.Site == place.Site)
                            place.IsBusy = true;
                    }
                }
            }
            else if (data.Type == TypeOperation.Delete)
            {
                var movie = await _repository.GetMovieByDateTimeAsync(data.DateTime);
                foreach (var place in movie.Places)
                {
                    foreach (var item in data.Places)
                    {
                        if (item.Row == place.Row && item.Site == place.Site)
                            place.IsBusy = false;
                    }
                }
            }
        }

        public async Task Consume(ConsumeContext<TicketShared> context)
        {
            var data = context.Message;
            if (data.Type == TypeOperation.Create)
            {
                var movie = await _repository.GetMovieByDateTimeAsync(data.DateTime);
                foreach (var place in movie.Places)
                {
                    foreach (var item in data.Places)
                    {
                        if (item.Row == place.Row && item.Site == place.Site)
                            place.IsBusy = true;
                    }
                }
            }
            else if (data.Type == TypeOperation.Delete)
            {
                var movie = await _repository.GetMovieByDateTimeAsync(data.DateTime);
                foreach (var place in movie.Places)
                {
                    foreach (var item in data.Places)
                    {
                        if (item.Row == place.Row && item.Site == place.Site)
                            place.IsBusy = false;
                    }
                }
            }
        }

        public async Task<MessageDetailsForCreateDTO> CreateMovieAsync(MovieForCreateDTO movieForCreateDTO)
        {
            var isTimeValid = await _repository.IsTimeValid(movieForCreateDTO.StartMovie, movieForCreateDTO.EndMovie);
            if (!isTimeValid)
                return new MessageDetailsForCreateDTO { StatusCode = 400, Movie = null };
            var entity = _mapper.Map<Movie>(movieForCreateDTO);

            for (int i = 0; i < entity.CountRows; i++)
            {
                for (int j = 0; j < entity.CountSites; j++)
                {
                    entity.Places.Add(new Place { Row = i + 1, Site = j + 1, IsBusy = false });
                }
            }

            _repository.CreateMovie(entity);
            var entityDto = _mapper.Map<MovieForReadDTO>(entity);

            Uri uri = new Uri("rabbitmq://localhost/MovieToReservationQueue?bind=true&queue=MovieToReservationQueue");
            var endPoint = await _bus.GetSendEndpoint(uri);
            var objBus = _mapper.Map<MovieShared>(entity);
            objBus.Type = TypeOperation.Create;
            await endPoint.Send(objBus);

            uri = new Uri("rabbitmq://localhost/MovieToTicketQueue?bind=true&queue=MovieToTicketQueue");
            endPoint = await _bus.GetSendEndpoint(uri);
            objBus = _mapper.Map<MovieShared>(entity);
            objBus.Type = TypeOperation.Create;
            await endPoint.Send(objBus);

            return new MessageDetailsForCreateDTO { StatusCode = 201, Movie = entityDto };
        }

        public async Task<MovieForReadDTO> GetMovieAsync(ObjectId id)
        {
            var movie = await _repository.GetMovieAsync(id);
            if (movie == null)
                return null;
            return _mapper.Map<MovieForReadDTO>(movie);
        }

        public async Task<long> GetMoviesCountAsync(MovieModelForSearchDTO searchModel)
        {
            var count = await _repository.GetMoviesCountAsync(searchModel);
            return count;
        }

        public async Task<IEnumerable<MovieForReadDTO>> GetMoviesPaginationAsync(int pageIndex, int pageSize, MovieModelForSearchDTO searchModel)
        {
            var movies = await _repository.GetAllMoviesPaginationAsync(pageIndex, pageSize, searchModel);
            return _mapper.Map<IEnumerable<MovieForReadDTO>>(movies);
        }

        public async Task<MessageDetailsDTO> UpdateMovieAsync(ObjectId id, MovieForUpdateDTO movieForUpdateDTO)
        {
            var movie = await _repository.GetMovieAsync(id);
            if (movie == null)
                return new MessageDetailsDTO { StatusCode = 404 };
            if (string.IsNullOrWhiteSpace(movieForUpdateDTO.Name))
                movie.Name = movieForUpdateDTO.Name;
            if (string.IsNullOrWhiteSpace(movieForUpdateDTO.Description))
                movie.Description = movieForUpdateDTO.Description;
            if(movieForUpdateDTO.Places.Count() != 0)
            {
                foreach (var place in movie.Places)
                {
                    foreach (var item in movieForUpdateDTO.Places)
                    {
                        if (item.Row == place.Row && item.Site == place.Site)
                            place.IsBusy = true;
                    }
                }
            }

            return new MessageDetailsDTO { StatusCode = 204 };
        }
    }
}
