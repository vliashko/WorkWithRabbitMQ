using Microsoft.EntityFrameworkCore;
using OrderMicroService.Contracts;
using OrderMicroService.Models;
using OrderMicroService.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderMicroService.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(RepositoryDbContext context) : base(context)
        {
        }

        public void CreateOrder(Order order)
        {
            Create(order);
        }

        public void DeleteOrder(Order order)
        {
            Delete(order);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersPaginationAsync(int pageIndex, int pageSize, OrderModelForSearchDTO searchModel, bool trackChanges)
        {
            return await FindByCondition(order =>
                (string.IsNullOrWhiteSpace(searchModel.Telephone) || order.Telephone.Contains(searchModel.Telephone)) &&
                (searchModel.DateTime == new DateTime() || searchModel.DateTime == order.PurchaseDateTime), trackChanges)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByDateTimeAsync(DateTime dateTime, bool trackChanges)
        {
            return await FindByCondition(order => order.PurchaseDateTime == dateTime, false).SingleOrDefaultAsync();
        }

        public async Task<Order> GetOrderAsync(int id, bool trackChanges)
        {
            return await FindByCondition(order => order.Id == id, trackChanges).SingleOrDefaultAsync();
        }

        public async Task<int> GetOrdersCountAsync(OrderModelForSearchDTO searchModel, bool trackChanges)
        {
            return await FindByCondition(order =>
                (string.IsNullOrWhiteSpace(searchModel.Telephone) || order.Telephone.Contains(searchModel.Telephone)) &&
                (searchModel.DateTime == new DateTime() || searchModel.DateTime == order.PurchaseDateTime), trackChanges)
                .CountAsync();
        }

        public async Task SaveAsync() => await context.SaveChangesAsync();
    }
}
