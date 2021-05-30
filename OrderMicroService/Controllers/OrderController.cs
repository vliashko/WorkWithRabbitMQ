using Microsoft.AspNetCore.Mvc;
using OrderMicroService.Contracts;
using OrderMicroService.Models.DataTransferObjects;
using OrderMicroService.Models.Pagination;
using System;
using System.Threading.Tasks;

namespace OrderMicroService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(int pageIndex = 1, int pageSize = 5, string dateTime = "", string telephone = "")
        {
            var searchModel = new OrderModelForSearchDTO() { Telephone = telephone, DateTime = DateTime.Parse(dateTime) };
            var tickets = await _service.GetOrdersPaginationAsync(pageIndex, pageSize, searchModel);
            var count = await _service.GetOrdersCountAsync(searchModel);
            PageViewModel pageViewModel = new PageViewModel(count, pageIndex, pageSize);
            ViewModel<OrderForReadDTO> viewModel = new ViewModel<OrderForReadDTO> { PageViewModel = pageViewModel, Objects = tickets };
            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var ticket = await _service.GetOrderAsync(id);
            if (ticket == null)
                return NotFound();
            return Ok(ticket);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _service.DeleteOrderAsync(id);
            return StatusCode(result.StatusCode, result.Message);
        }
    }
}
