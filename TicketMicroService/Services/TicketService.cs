using AutoMapper;
using MassTransit;
using SharedModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketMicroService.Contracts;
using TicketMicroService.Models;
using TicketMicroService.Models.DataTransferObjects;

namespace TicketMicroService.Services
{
    public class TicketService : ITicketService, IConsumer<MovieShared>
    {
        private readonly ITicketRepository _repository;
        private readonly IMovieRepository _movieRepository;
        private readonly IBus _bus;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository repository, IMovieRepository movieRepository, IBus bus, IMapper mapper)
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

        public Task<MessageDetailsForCreateDTO> CreateTicketAsync(TicketForCreateDTO TicketForCreateDTO)
        {
            throw new System.NotImplementedException();
        }

        public Task<TicketForReadDTO> GetTicketAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetTicketsCountAsync(TicketModelForSearchDTO searchModel)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<TicketForReadDTO>> GetTicketsPaginationAsync(int pageIndex, int pageSize, TicketModelForSearchDTO searchModel)
        {
            throw new System.NotImplementedException();
        }
    }
}
