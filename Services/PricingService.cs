public class PricingService
{
    private readonly SeatRepository _seatRepo;
    private readonly CategoryRepository _categoryRepo;

    public PricingService(SeatRepository seatRepo, CategoryRepository categoryRepo)
    {
        _seatRepo = seatRepo;
        _categoryRepo = categoryRepo;
    }

    public decimal CalculatePrice(int seatId)
    {
        var seat = _seatRepo.GetById(seatId);
        var category = _categoryRepo.GetById(seat.CategoryId);
        var demandCoefficient = CalculateDemandCoefficient(seat.EventId);
        
        return category.BasePrice * category.Multiplier * demandCoefficient;
    }

    public decimal CalculateDemandCoefficient(int eventId)
    {
        var soldSeats = _seatRepo.GetSoldSeatsCount(eventId);
        var totalSeats = _seatRepo.GetTotalSeatsCount(eventId);
        
        if (totalSeats == 0) return 1.0m;
        
        var soldRatio = (decimal)soldSeats / totalSeats;
        
        if (soldRatio >= 0.9m) return 2.0m;
        if (soldRatio >= 0.7m) return 1.5m;
        if (soldRatio >= 0.5m) return 1.2m;
        return 1.0m;
    }

    public decimal CalculateRefundCommission(int ticketId)
    {
        var ticketRepo = new TicketRepository(new DatabaseService(""));
        var eventRepo = new EventRepository(new DatabaseService(""));
        
        var ticket = ticketRepo.GetById(ticketId);
        var evt = eventRepo.GetById(ticket.EventId);
        
        var hoursUntilEvent = (evt.Date - DateTime.Now).TotalHours;
        
        if (hoursUntilEvent > 72) return ticket.Price * 0.05m;
        if (hoursUntilEvent > 24) return ticket.Price * 0.15m;
        if (hoursUntilEvent > 2) return ticket.Price * 0.30m;
        return ticket.Price * 0.50m;
    }
}