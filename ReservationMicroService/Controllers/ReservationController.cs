using Microsoft.AspNetCore.Mvc;
using ReservationMicroService.Contracts;
using ReservationMicroService.Models.DataTransferObjects;
using ReservationMicroService.Models.Pagination;
using System;
using System.Threading.Tasks;

namespace ReservationMicroService.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _service;

        public ReservationController(IReservationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations(int pageIndex = 1, int pageSize = 5, string dateTime = "", string telephone = "")
        {
            var searchModel = new ReservationModelForSearchDTO() { Telephone = telephone, DateTime = DateTime.Parse(dateTime) };
            var Reservations = await _service.GetReservationsPaginationAsync(pageIndex, pageSize, searchModel);
            var count = await _service.GetReservationsCountAsync(searchModel);
            PageViewModel pageViewModel = new PageViewModel(count, pageIndex, pageSize);
            ViewModel<ReservationForReadDTO> viewModel = new ViewModel<ReservationForReadDTO> { PageViewModel = pageViewModel, Objects = Reservations };
            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var Reservation = await _service.GetReservationAsync(id);
            if (Reservation == null)
                return NotFound();
            return Ok(Reservation);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation(ReservationForCreateDTO ReservationDto)
        {
            if (ReservationDto == null)
                return BadRequest();
            var ReservationResult = await _service.CreateReservationAsync(ReservationDto);
            if (ReservationResult.StatusCode == 400 && ReservationResult.Reservation == null)
                return StatusCode(ReservationResult.StatusCode, "These seats have already been purchased / booked.");
            if (ReservationResult.StatusCode == 406)
                return StatusCode(ReservationResult.StatusCode, "Exception with time of movie or with places.");
            return StatusCode(ReservationResult.StatusCode, ReservationResult.Reservation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var result = await _service.DeleteReservationAsync(id);
            return StatusCode(result.StatusCode, result.Message);
        }

        [HttpDelete("time/{dateTime}")]
        public async Task<IActionResult> DeleteAllUnboughtReservationsForMovie(DateTime dateTime)
        {
            var result = await _service.DeleteAllUnboughtReservationsForMovie(dateTime);
            return StatusCode(result.StatusCode, result.Message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, ReservationForUpdateDTO ReservationDto)
        {
            if (ReservationDto == null)
                return BadRequest();
            var result = await _service.UpdateReservationAsync(id, ReservationDto);
            return StatusCode(result.StatusCode, result.Message);
        }
    }
}
