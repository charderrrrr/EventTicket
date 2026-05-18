public class EventController
{
    private readonly EventRepository _eventRepo;
    private readonly VenueLayoutService _venueLayoutService;

    public EventController(EventRepository eventRepo, VenueLayoutService venueLayoutService)
    {
        _eventRepo = eventRepo;
        _venueLayoutService = venueLayoutService;
    }

    public Event CreateEvent(string name, DateTime date, int venueId)
    {
        var evt = new Event
        {
            Name = name,
            Date = date,
            VenueId = venueId,
            Status = "active"
        };
        
        var created = _eventRepo.Create(evt);
        _venueLayoutService.GenerateSeatsForEvent(created.Id);
        
        return created;
    }

    public IEnumerable<Event> GetActiveEvents()
    {
        return _eventRepo.GetAll();
    }

    public Event GetEvent(int id)
    {
        return _eventRepo.GetById(id);
    }
}