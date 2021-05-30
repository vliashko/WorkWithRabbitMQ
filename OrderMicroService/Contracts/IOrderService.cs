using OrderMicroService.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderMicroService.Contracts
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderForReadDTO>> GetOrdersPaginationAsync(int pageIndex, int pageSize, OrderModelForSearchDTO searchModel);
        Task<int> GetOrdersCountAsync(OrderModelForSearchDTO searchModel);
        Task<OrderForReadDTO> GetOrderAsync(int id);
        Task<OrderForReadDTO> GetOrderByDateTimeAsync(DateTime dateTime);
        Task<MessageDetailsDTO> DeleteOrderAsync(int id);
    }
}
