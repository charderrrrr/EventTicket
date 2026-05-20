using Microsoft.AspNetCore.Mvc;
using EventTicket.Data.Repositories;

namespace EventTicket.Controllers
{
    [ApiController]
    [Route("api/venues")]
    public class VenueController : ControllerBase
    {
        private readonly EventTicketModule _module;

        public VenueController(EventTicketModule module)
        {
            _module = module;
        }

        [HttpGet]
        public IActionResult GetVenues()
        {
            using var connection = _module.CreateConnection();
            var repo = new VenueRepository(connection);
            return Ok(repo.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetVenue(int id)
        {
            using var connection = _module.CreateConnection();
            var repo = new VenueRepository(connection);
            var venue = repo.GetById(id);
            if (venue == null)
                return NotFound(new { error = "Зал не найден" });
            return Ok(venue);
        }
    }
}