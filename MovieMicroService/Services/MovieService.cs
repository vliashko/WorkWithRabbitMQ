using AutoMapper;
using MassTransit;
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
    public class MovieService : IMovieService, IConsumer<TicketShared>
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

        public async Task Consume(ConsumeContext<TicketShared> context)
        {
            var data = context.Message;
            if(data.Type == TypeOperation.Create)
            {
                var movie = await _repository.GetMovieByDateTimeAsync(data.DateTime, true);
                foreach (var place in movie.Places)
                {
                    foreach (var item in data.Places)
                    {
                        if (item.Row == place.Row && item.Site == place.Site)
                            place.IsBusy = true;
                    }
                }
                await _repository.SaveAsync();
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
            await _repository.SaveAsync();
            var entityDto = _mapper.Map<MovieForReadDTO>(entity);

            Uri uri = new Uri("rabbitmq://localhost/movieQueue?bind=true&queue=movieQueue");
            var endPoint = await _bus.GetSendEndpoint(uri);
            var objBus = _mapper.Map<MovieShared>(entity);
            objBus.Type = TypeOperation.Create;
            await endPoint.Send(objBus);

            return new MessageDetailsForCreateDTO { StatusCode = 201, Movie = entityDto };
        }

        public async Task<MovieForReadDTO> GetMovieAsync(int id)
        {
            var movie = await _repository.GetMovieAsync(id, false);
            if (movie == null)
                return null;
            return _mapper.Map<MovieForReadDTO>(movie);
        }

        public async Task<int> GetMoviesCountAsync(MovieModelForSearchDTO searchModel)
        {
            var count = await _repository.GetMoviesCountAsync(searchModel, false);
            return count;
        }

        public async Task<IEnumerable<MovieForReadDTO>> GetMoviesPaginationAsync(int pageIndex, int pageSize, MovieModelForSearchDTO searchModel)
        {
            var movies = await _repository.GetAllMoviesPaginationAsync(pageIndex, pageSize, searchModel, false);
            return _mapper.Map<IEnumerable<MovieForReadDTO>>(movies);
        }

        public async Task<MessageDetailsDTO> UpdateMovieAsync(int id, MovieForUpdateDTO movieForUpdateDTO)
        {
            var movie = await _repository.GetMovieAsync(id, true);
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
            await _repository.SaveAsync();

            Uri uri = new Uri("rabbitmq://localhost/movieQueue?bind=true&queue=movieQueue");
            var endPoint = await _bus.GetSendEndpoint(uri);
            var objBus = _mapper.Map<MovieShared>(movie);
            objBus.Type = TypeOperation.Edit;
            await endPoint.Send(objBus);

            return new MessageDetailsDTO { StatusCode = 204 };
        }
    }
}
