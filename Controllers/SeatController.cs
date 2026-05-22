// SeatController - API контроллер управления местами.
// Обрабатывает запросы на получение схемы зала для события, информации о конкретном месте,
// расчет текущей цены с учетом коэффициента спроса и проверку доступности места.
// Все эндпоинты привязаны к конкретному событию через eventId.

using Microsoft.AspNetCore.Mvc;
using EventTicket.Data.Repositories;
using EventTicket.Services;

namespace EventTicket.Controllers
{
    [ApiController]
    [Route("api/events/{eventId}/seats")]
    public class SeatController : ControllerBase
    {
        private readonly EventTicketModule _module;

        public SeatController(EventTicketModule module)
        {
            _module = module;
        }

        [HttpGet]
        public IActionResult GetEventSeats(int eventId)
        {
            using var connection = _module.CreateConnection();
            var layoutService = new VenueLayoutService(connection);
            var pricingService = new PricingService(connection);
            var seatRepo = new SeatRepository(connection);

            var seats = layoutService.GetEventSeats(eventId);
            var coefficient = pricingService.CalculateDemandCoefficient(eventId);
            var soldSeats = seatRepo.GetSoldSeatsCount(eventId);
            var totalSeats = seatRepo.GetTotalAvailableSeats(eventId);

            return Ok(new
            {
                seats = seats,
                demand = new
                {
                    coefficient = coefficient,
                    soldSeats = soldSeats,
                    totalSeats = totalSeats
                }
            });
        }

        [HttpGet("{seatId}")]
        public IActionResult GetSeatInfo(int eventId, int seatId)
        {
            using var connection = _module.CreateConnection();
            var layoutService = new VenueLayoutService(connection);
            var seat = layoutService.GetSeatInfo(seatId);
            if (seat.EventId != eventId)
                return NotFound(new { error = "Место не принадлежит этому событию" });
            return Ok(seat);
        }

        [HttpGet("{seatId}/price")]
        public IActionResult GetSeatPrice(int eventId, int seatId)
        {
            using var connection = _module.CreateConnection();
            var pricingService = new PricingService(connection);
            var price = pricingService.CalculatePrice(seatId);
            return Ok(new { price = price });
        }
    }
}