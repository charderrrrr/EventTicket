using System.Text.Json;

public class VenueLayoutService
{
    private readonly VenueRepository _venueRepo;
    private readonly SeatRepository _seatRepo;
    private readonly EventRepository _eventRepo;

    public VenueLayoutService(VenueRepository venueRepo, SeatRepository seatRepo, EventRepository eventRepo)
    {
        _venueRepo = venueRepo;
        _seatRepo = seatRepo;
        _eventRepo = eventRepo;
    }

    public void GenerateSeatsForEvent(int eventId)
    {
        var evt = _eventRepo.GetById(eventId);
        var venue = _venueRepo.GetById(evt.VenueId);
        var blockedSeats = JsonSerializer.Deserialize<List<BlockedSeat>>(venue.BlockedSeats);
        var categories = new CategoryRepository(_venueRepo as dynamic).GetAll();
        
        var seats = new List<Seat>();
        
        for (int row = 1; row <= venue.Rows; row++)
        {
            for (int num = 1; num <= venue.SeatsPerRow; num++)
            {
                bool isBlocked = blockedSeats.Any(b => b.Row == row && b.Number == num);
                
                var seat = new Seat
                {
                    EventId = eventId,
                    Row = row,
                    Number = num,
                    CategoryId = DetermineCategory(row, venue.Rows),
                    Status = isBlocked ? "blocked" : "available"
                };
                
                seats.Add(seat);
            }
        }
        
        _seatRepo.CreateBatch(seats);
    }

    private int DetermineCategory(int row, int totalRows)
    {
        if (row <= 3) return 3;
        if (row <= 7) return 1;
        return 2;
    }

    public string[,] GetVenueLayout(int eventId)
    {
        var evt = _eventRepo.GetById(eventId);
        var venue = _venueRepo.GetById(evt.VenueId);
        var seats = _seatRepo.GetByEventId(eventId).ToList();
        
        var layout = new string[venue.Rows, venue.SeatsPerRow];
        
        foreach (var seat in seats)
        {
            layout[seat.Row - 1, seat.Number - 1] = seat.Status;
        }
        
        return layout;
    }

    private class BlockedSeat
    {
        public int Row { get; set; }
        public int Number { get; set; }
    }
}