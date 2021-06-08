using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MovieMicroService.Contracts;
using MovieMicroService.Models.DataTransferObjects;
using MovieMicroService.Models.Pagination;
using System;
using System.Threading.Tasks;

namespace MovieMicroService.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MovieContoller : ControllerBase
    {
        private readonly IMovieService _service;

        public MovieContoller(IMovieService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies(int pageIndex = 1, int pageSize = 5, string dateTime = "", string name = "")
        {
            var searchModel = new MovieModelForSearchDTO() { Name = name, StartMovie = DateTime.Parse(dateTime) };
            var movies = await _service.GetMoviesPaginationAsync(pageIndex, pageSize, searchModel);
            var count = await _service.GetMoviesCountAsync(searchModel);
            PageViewModel pageViewModel = new PageViewModel(count, pageIndex, pageSize);
            ViewModel<MovieForReadDTO> viewModel = new ViewModel<MovieForReadDTO> { PageViewModel = pageViewModel, Objects = movies };
            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(string id)
        {
            var movie = await _service.GetMovieAsync(new ObjectId(id));
            if (movie == null)
                return NotFound();
            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie(MovieForCreateDTO movieDto)
        {
            if (movieDto == null)
                return BadRequest();
            var ticketResult = await _service.CreateMovieAsync(movieDto);
            if (ticketResult.StatusCode == 400 && ticketResult.Movie == null)
                return StatusCode(ticketResult.StatusCode, "Time is not valid.");
            return StatusCode(ticketResult.StatusCode, ticketResult.Movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(string id, MovieForUpdateDTO movieDto)
        {
            if (movieDto == null)
                return BadRequest();
            var result = await _service.UpdateMovieAsync(new ObjectId(id), movieDto);
            return StatusCode(result.StatusCode, result.Message);
        }
    }
}
