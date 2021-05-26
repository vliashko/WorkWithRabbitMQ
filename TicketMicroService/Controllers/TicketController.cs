using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TicketMicroService.Contracts;
using TicketMicroService.Models.DataTransferObjects;
using TicketMicroService.Models.Pagination;

namespace TicketMicroService.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _service;

        public TicketController(ITicketService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets(int pageIndex = 1, int pageSize = 5, string dateTime = "", string telephone = "")
        {
            var searchModel = new TicketModelForSearchDTO() { Telephone = telephone, DateTime = DateTime.Parse(dateTime) };
            var tickets = await _service.GetTicketsPaginationAsync(pageIndex, pageSize, searchModel);
            var count = await _service.GetTicketsCountAsync(searchModel);
            PageViewModel pageViewModel = new PageViewModel(count, pageIndex, pageSize);
            ViewModel<TicketForReadDTO> viewModel = new ViewModel<TicketForReadDTO> { PageViewModel = pageViewModel, Objects = tickets };
            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var ticket = await _service.GetTicketAsync(id);
            if (ticket == null)
                return NotFound();
            return Ok(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(TicketForCreateDTO ticketDto)
        {
            if (ticketDto == null)
                return BadRequest();
            var ticketResult = await _service.CreateTicketAsync(ticketDto);
            if(ticketResult.StatusCode == 400 && ticketResult.Ticket == null)
                return StatusCode(ticketResult.StatusCode, "These seats have already been purchased / booked.");
            if (ticketResult.StatusCode == 406)
                return StatusCode(ticketResult.StatusCode, "Identical places selected.");
            return StatusCode(ticketResult.StatusCode, ticketResult.Ticket);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var result = await _service.DeleteTicketAsync(id);
            return StatusCode(result.StatusCode, result.Message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, TicketForUpdateDTO ticketDto)
        {
            if (ticketDto == null)
                return BadRequest();
            var result = await _service.UpdateTicketAsync(id, ticketDto);
            return StatusCode(result.StatusCode, result.Message);
        }
    }
}
