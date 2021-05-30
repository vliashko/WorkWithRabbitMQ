using AutoMapper;
using MassTransit;
using OrderMicroService.Contracts;
using OrderMicroService.Models;
using OrderMicroService.Models.DataTransferObjects;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderMicroService.Services
{
    public class OrderService : IOrderService, IConsumer<TicketShared>, IConsumer<ReservationShared>
    {
        private readonly IOrderRepository _repository;
        private readonly IBus _bus;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository repository, IBus bus, IMapper mapper)
        {
            _repository = repository;
            _bus = bus;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<TicketShared> context)
        {
            var data = context.Message;
            var guid = Guid.NewGuid();
            var entity = new Order
            {
                Email = data.Email,
                DateTime = data.DateTime,
                PurchaseDateTime = DateTime.Now,
                TotalTickets = data.Places.Count(),
                PaymentCode = guid
            };
            _repository.CreateOrder(entity);
            await _repository.SaveAsync();

            Uri uri = new Uri("rabbitmq://localhost/OrderToTicketQueue?bind=true&queue=OrderToTicketQueue");
            var endPoint = await _bus.GetSendEndpoint(uri);
            var objBus = _mapper.Map<OrderToTicketShared>(entity);
            objBus.Type = TypeOperation.Create;
            await endPoint.Send(objBus);
        }

        public async Task Consume(ConsumeContext<ReservationShared> context)
        {
            var data = context.Message;
            Guid guid = Guid.NewGuid();
            var orderToCreate = new Order
            {
                Telephone = data.Telephone,
                DateTime = data.DateTime,
                PurchaseDateTime = DateTime.Now,
                TotalTickets = data.Places.Count(),
                PaymentCode = guid
            };
            _repository.CreateOrder(orderToCreate);
            await _repository.SaveAsync();

            Uri uri = new Uri("rabbitmq://localhost/OrderToReservationQueue?bind=true&queue=OrderToReservationQueue");
            var endPoint = await _bus.GetSendEndpoint(uri);
            var objBus = _mapper.Map<OrderToReservationShared>(orderToCreate);
            objBus.Type = TypeOperation.Create;
            await endPoint.Send(objBus);
        }

        public async Task<MessageDetailsDTO> DeleteOrderAsync(int id)
        {
            var order = await _repository.GetOrderAsync(id, false);
            if (order == null)
                return new MessageDetailsDTO { StatusCode = 404, Message = $"Ticket with id: {id} doesn't exist in the database" };

            Uri uri = new Uri("rabbitmq://localhost/orderQueue?bind=true&queue=orderQueue");
            var endPoint = await _bus.GetSendEndpoint(uri);
            var objBus = _mapper.Map<OrderToReservationShared>(order);
            objBus.Type = TypeOperation.Delete;
            await endPoint.Send(objBus);

            _repository.DeleteOrder(order);
            await _repository.SaveAsync();
            return new MessageDetailsDTO { StatusCode = 204 };
        }

        public async Task<OrderForReadDTO> GetOrderAsync(int id)
        {
            var order = await _repository.GetOrderAsync(id, false);
            return _mapper.Map<OrderForReadDTO>(order);
        }

        public async Task<OrderForReadDTO> GetOrderByDateTimeAsync(DateTime dateTime)
        {
            var order = await _repository.GetOrderByDateTimeAsync(dateTime, false);
            return _mapper.Map<OrderForReadDTO>(order);
        }

        public async Task<int> GetOrdersCountAsync(OrderModelForSearchDTO searchModel)
        {
            var count = await _repository.GetOrdersCountAsync(searchModel, false);
            return count;
        }

        public async Task<IEnumerable<OrderForReadDTO>> GetOrdersPaginationAsync(int pageIndex, int pageSize, OrderModelForSearchDTO searchModel)
        {
            var orders = await _repository.GetAllOrdersPaginationAsync(pageIndex, pageSize, searchModel, false);
            return _mapper.Map<IEnumerable<OrderForReadDTO>>(orders);
        }
    }
}
