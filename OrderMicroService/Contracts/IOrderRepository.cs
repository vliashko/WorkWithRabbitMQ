using OrderMicroService.Models;
using OrderMicroService.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderMicroService.Contracts
{
    public interface IOrderRepository
    {
        void CreateOrder(Order order);
        void DeleteOrder(Order order);
        Task<Order> GetOrderAsync(int id, bool trackChanges);
        Task<Order> GetOrderByDateTimeAsync(DateTime dateTime, bool trackChanges);
        Task<IEnumerable<Order>> GetAllOrdersPaginationAsync(int pageIndex, int pageSize, OrderModelForSearchDTO searchModel, bool trackChanges);
        Task<int> GetOrdersCountAsync(OrderModelForSearchDTO searchModel, bool trackChanges);
        Task SaveAsync();
    }
}
