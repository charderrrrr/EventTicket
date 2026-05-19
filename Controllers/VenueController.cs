using Microsoft.AspNetCore.Mvc;

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
            var venues = _module.VenueRepository.GetAll();
            return Ok(venues);
        }

        [HttpGet("{id}")]
        public IActionResult GetVenue(int id)
        {
            var venue = _module.VenueRepository.GetById(id);
            if (venue == null)
                return NotFound(new { error = "Зал не найден" });
            return Ok(venue);
        }
    }
}