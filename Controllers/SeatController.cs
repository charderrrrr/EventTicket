public class SeatController
{
    private readonly SeatRepository _seatRepo;
    private readonly VenueLayoutService _venueLayoutService;

    public SeatController(SeatRepository seatRepo, VenueLayoutService venueLayoutService)
    {
        _seatRepo = seatRepo;
        _venueLayoutService = venueLayoutService;
    }

    public string[,] GetEventLayout(int eventId)
    {
        return _venueLayoutService.GetVenueLayout(eventId);
    }

    public IEnumerable<Seat> GetAvailableSeats(int eventId)
    {
        return _seatRepo.GetByEventId(eventId).Where(s => s.Status == "available");
    }
}