using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using EventTicket.Data.Repositories;
using EventTicket.Data;
using EventTicket.Models;

namespace EventTicket.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly EventRepository _eventRepository;
        private readonly DatabaseService _databaseService;

        public EventController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            var connection = databaseService.CreateConnection();
            _eventRepository = new EventRepository(connection);
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            using var connection = _databaseService.CreateConnection();
            var repo = new EventRepository(connection);
            var events = repo.GetAll();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public IActionResult GetEvent(int id)
        {
            using var connection = _databaseService.CreateConnection();
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

            using var connection = _databaseService.CreateConnection();
            var repo = new EventRepository(connection);
            
            var evt = Event.Create(request.Name, request.Date, request.VenueId);
            var created = repo.Create(evt);
            
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