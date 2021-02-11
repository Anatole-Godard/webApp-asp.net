using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly MvcMovieContext _context;

        public APIController(MvcMovieContext context)
        {
            _context = context;
        }

        // GET: api/Movie
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> GetMovie()
        {
            return await _context.Movie
                .Select(x => MovieToDTO(x))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDTO>> GetMovie(long id)
        {
            var movie = await _context.Movie.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return MovieToDTO(movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(long id, MovieDTO movieDTO)
        {
            if (id != movieDTO.Id)
            {
                return BadRequest();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            movie.Title = movieDTO.Title;
            movie.ReleaseDate = movieDTO.ReleaseDate;
            movie.Price = movieDTO.Price;
            movie.Genre = movieDTO.Genre;
            movie.Rating = movieDTO.Rating;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!MovieExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<MovieDTO>> CreateMovie(MovieDTO movieDTO)
        {
            var movie = new Movie
            {
                Id = movieDTO.Id,
                Title = movieDTO.Title,
                ReleaseDate = movieDTO.ReleaseDate,
                Price = movieDTO.Price,
                Genre = movieDTO.Genre,
                Rating = movieDTO.Rating,
            };

            _context.Movie.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMovie),
                new { id = movie.Id },
                MovieToDTO(movie));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(long id)
        {
            var todoItem = await _context.Movie.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.Movie.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(long id) =>
             _context.Movie.Any(e => e.Id == id);

        private static MovieDTO MovieToDTO(Movie movie) =>
            new MovieDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                ReleaseDate = movie.ReleaseDate,
                Price = movie.Price,
                Genre = movie.Genre,
                Rating = movie.Rating,
            };
    }
}