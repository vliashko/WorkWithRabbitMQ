using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> GetTickets(int pageIndex = 1, int pageSize = 5, string dateTime = "", string email = "")
        {
            var searchModel = new TicketModelForSearchDTO { Email = email, DateTime = DateTime.Parse(dateTime) };
            var tickets = await _service.GetTicketsPaginationAsync(pageIndex, pageSize, searchModel);
            var count = await _service.GetTicketsCountAsync(searchModel);
            PageViewModel pageViewModel = new PageViewModel(count, pageIndex, pageSize);
            ViewModel<TicketForReadDTO> viewModel = new ViewModel<TicketForReadDTO> { PageViewModel = pageViewModel, Objects = tickets };
            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var Reservation = await _service.GetTicketAsync(id);
            if (Reservation == null)
                return NotFound();
            return Ok(Reservation);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(TicketForCreateDTO ticketDto)
        {
            if (ticketDto == null)
                return BadRequest();
            var ticketResult = await _service.CreateTicketAsync(ticketDto);
            if (ticketResult.StatusCode == 400 && ticketResult.Ticket == null)
                return StatusCode(ticketResult.StatusCode, "These seats have already been purchased / booked.");
            if (ticketResult.StatusCode == 406)
                return StatusCode(ticketResult.StatusCode, "Exception with time of movie or with places.");
            return StatusCode(ticketResult.StatusCode, ticketResult.Ticket);
        }
    }
}
