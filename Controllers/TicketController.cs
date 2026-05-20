using Microsoft.AspNetCore.Mvc;
using EventTicket.Data.Repositories;
using EventTicket.Services;

namespace EventTicket.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketController : ControllerBase
    {
        private readonly EventTicketModule _module;

        public TicketController(EventTicketModule module)
        {
            _module = module;
        }

        [HttpPost("purchase")]
        public IActionResult PurchaseTicket([FromBody] PurchaseTicketRequest request)
        {
            try
            {
                using var connection = _module.CreateConnection();
                var bookingService = new BookingService(connection);
                var ticket = bookingService.PurchaseTicket(request.UserId, request.EventId, request.SeatId);
                return Ok(ticket);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{ticketId}/refund")]
        public IActionResult RefundTicket(int ticketId)
        {
            try
            {
                using var connection = _module.CreateConnection();
                var refundService = new RefundService(connection);
                var result = refundService.RefundTicket(ticketId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{ticketId}/refund-commission")]
        public IActionResult GetRefundCommission(int ticketId)
        {
            using var connection = _module.CreateConnection();
            var ticketRepo = new TicketRepository(connection);
            var eventRepo = new EventRepository(connection);
            var pricingService = new PricingService(connection);

            var ticket = ticketRepo.GetById(ticketId);
            if (ticket == null)
                return NotFound(new { error = "Билет не найден" });

            var evt = eventRepo.GetById(ticket.EventId);
            if (evt == null)
                return NotFound(new { error = "Событие не найдено" });

            var commission = pricingService.CalculateRefundCommission(ticketId, ticket.Price, evt.Date);
            return Ok(new
            {
                commission = commission,
                refundAmount = ticket.Price - commission
            });
        }

        [HttpGet]
        public IActionResult GetUserTickets([FromQuery] int userId = 1)
        {
            using var connection = _module.CreateConnection();
            var ticketRepo = new TicketRepository(connection);
            var tickets = ticketRepo.GetByUserId(userId);
            return Ok(tickets);
        }
    }

    public class PurchaseTicketRequest
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int SeatId { get; set; }
    }
}