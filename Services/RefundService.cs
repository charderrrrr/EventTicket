public class RefundService
{
    private readonly TicketRepository _ticketRepo;
    private readonly SeatRepository _seatRepo;
    private readonly PricingService _pricingService;

    public RefundService(TicketRepository ticketRepo, SeatRepository seatRepo, PricingService pricingService)
    {
        _ticketRepo = ticketRepo;
        _seatRepo = seatRepo;
        _pricingService = pricingService;
    }

    public decimal RefundTicket(int ticketId)
    {
        var ticket = _ticketRepo.GetById(ticketId);
        
        if (ticket.Status != "active")
            throw new InvalidOperationException("Ticket is not active");
        
        var commission = _pricingService.CalculateRefundCommission(ticketId);
        var refundAmount = ticket.Price - commission;
        
        _ticketRepo.UpdateStatus(ticketId, "refunded");
        _seatRepo.UpdateStatus(ticket.SeatId, "available");
        
        return refundAmount;
    }

    public IEnumerable<Ticket> GetUserTickets(int userId)
    {
        return _ticketRepo.GetByUserId(userId);
    }
}