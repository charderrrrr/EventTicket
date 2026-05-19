using Microsoft.AspNetCore.Mvc;

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
                var ticket = _module.BookingService.PurchaseTicket(request.UserId, request.EventId, request.SeatId);
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
                var result = _module.RefundService.RefundTicket(ticketId);
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
            var ticket = _module.TicketRepository.GetById(ticketId);
            if (ticket == null)
                return NotFound(new { error = "Билет не найден" });

            var evt = _module.EventRepository.GetById(ticket.EventId);
            if (evt == null)
                return NotFound(new { error = "Событие не найдено" });

            var commission = _module.PricingService.CalculateRefundCommission(ticketId, ticket.Price, evt.Date);
            return Ok(new
            {
                commission = commission,
                refundAmount = ticket.Price - commission
            });
        }

        [HttpGet]
        public IActionResult GetUserTickets([FromQuery] int userId = 1)
        {
            var tickets = _module.TicketRepository.GetByUserId(userId);
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