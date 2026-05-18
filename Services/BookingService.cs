public class BookingService
{
    private readonly SeatRepository _seatRepo;
    private readonly TicketRepository _ticketRepo;
    private readonly PricingService _pricingService;

    public BookingService(SeatRepository seatRepo, TicketRepository ticketRepo, PricingService pricingService)
    {
        _seatRepo = seatRepo;
        _ticketRepo = ticketRepo;
        _pricingService = pricingService;
    }

    public Ticket PurchaseTicket(int userId, int seatId)
    {
        var seat = _seatRepo.GetById(seatId);
        
        if (seat.Status != "available")
            throw new InvalidOperationException("Seat is not available");
        
        var price = _pricingService.CalculatePrice(seatId);
        
        _seatRepo.UpdateStatus(seatId, "sold");
        
        var ticket = new Ticket
        {
            UserId = userId,
            EventId = seat.EventId,
            SeatId = seatId,
            PurchaseDate = DateTime.Now,
            Price = price,
            Status = "active"
        };
        
        return _ticketRepo.Create(ticket);
    }

    public bool IsSeatAvailable(int seatId)
    {
        var seat = _seatRepo.GetById(seatId);
        return seat.Status == "available";
    }
}