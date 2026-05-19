using Microsoft.AspNetCore.Mvc;

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
            var seats = _module.VenueLayoutService.GetEventSeats(eventId);
            var coefficient = _module.PricingService.CalculateDemandCoefficient(eventId);
            var soldSeats = _module.SeatRepository.GetSoldSeatsCount(eventId);
            var totalSeats = _module.SeatRepository.GetTotalAvailableSeats(eventId);

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
            var seat = _module.VenueLayoutService.GetSeatInfo(seatId);
            if (seat.EventId != eventId)
                return NotFound(new { error = "Место не принадлежит этому событию" });
            return Ok(seat);
        }

        [HttpGet("{seatId}/availability")]
        public IActionResult CheckAvailability(int eventId, int seatId)
        {
            var isAvailable = _module.BookingService.IsSeatAvailable(seatId);
            return Ok(new { available = isAvailable });
        }

        [HttpGet("{seatId}/price")]
        public IActionResult GetSeatPrice(int eventId, int seatId)
        {
            var price = _module.PricingService.CalculatePrice(seatId);
            return Ok(new { price = price });
        }
    }
}