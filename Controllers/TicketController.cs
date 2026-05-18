public class TicketController
{
    private readonly BookingService _bookingService;
    private readonly RefundService _refundService;

    public TicketController(BookingService bookingService, RefundService refundService)
    {
        _bookingService = bookingService;
        _refundService = refundService;
    }

    public Ticket PurchaseTicket(int userId, int seatId)
    {
        return _bookingService.PurchaseTicket(userId, seatId);
    }

    public decimal RefundTicket(int ticketId)
    {
        return _refundService.RefundTicket(ticketId);
    }

    public IEnumerable<Ticket> GetUserTickets(int userId)
    {
        return _refundService.GetUserTickets(userId);
    }

    public bool CheckSeatAvailability(int seatId)
    {
        return _bookingService.IsSeatAvailable(seatId);
    }
}