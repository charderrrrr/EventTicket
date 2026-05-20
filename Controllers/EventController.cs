using Microsoft.AspNetCore.Mvc;
using EventTicket.Data.Repositories;
using EventTicket.Models;

namespace EventTicket.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly EventTicketModule _module;

        public EventController(EventTicketModule module)
        {
            _module = module;
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            using var connection = _module.CreateConnection();
            var repo = new EventRepository(connection);
            return Ok(repo.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetEvent(int id)
        {
            using var connection = _module.CreateConnection();
            var repo = new EventRepository(connection);
            var evt = repo.GetById(id);
            if (evt == null)
                return NotFound(new { error = "Событие не найдено" });
            return Ok(evt);
        }

        [HttpPost]
        public IActionResult CreateEvent([FromBody] CreateEventRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                return BadRequest(new { error = "Название события обязательно" });

            using var connection = _module.CreateConnection();
            var repo = new EventRepository(connection);
            var layoutService = new Services.VenueLayoutService(connection);

            var evt = Event.Create(request.Name, request.Date, request.VenueId);
            var created = repo.Create(evt);
            layoutService.GenerateSeatsForEvent(created.Id);

            return Ok(created);
        }
    }

    public class CreateEventRequest
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int VenueId { get; set; }
    }
}