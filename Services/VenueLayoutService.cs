using System.Data;
using EventTicket.Data.Repositories;
using EventTicket.Models;
using EventTicket.Models.Enums;

namespace EventTicket.Services
{
    public class VenueLayoutService
    {
        private readonly IDbConnection _connection;
        private readonly VenueRepository _venueRepository;
        private readonly SeatRepository _seatRepository;
        private readonly EventRepository _eventRepository;
        private readonly CategoryRepository _categoryRepository;

        public VenueLayoutService(IDbConnection connection)
        {
            _connection = connection;
            _venueRepository = new VenueRepository(connection);
            _seatRepository = new SeatRepository(connection);
            _eventRepository = new EventRepository(connection);
            _categoryRepository = new CategoryRepository(connection);
        }

        public void GenerateSeatsForEvent(int eventId)
        {
            var evt = _eventRepository.GetById(eventId);
            if (evt == null)
                throw new InvalidOperationException("Событие не найдено");

            var venue = _venueRepository.GetById(evt.VenueId);
            if (venue == null)
                throw new InvalidOperationException("Зал не найден");

            var categories = _categoryRepository.GetAll().ToList();
            var seats = new List<Seat>();

            for (int row = 1; row <= venue.Rows; row++)
            {
                for (int number = 1; number <= venue.SeatsPerRow; number++)
                {
                    var isBlocked = venue.IsSeatBlocked(row, number);
                    
                    int categoryId;
                    if (row <= 3)
                        categoryId = categories.First(c => c.Name == "VIP").Id;
                    else if (row <= 7)
                        categoryId = categories.First(c => c.Name == "Parter").Id;
                    else
                        categoryId = categories.First(c => c.Name == "Balcony").Id;

                    var seat = Seat.Create(
                        eventId,
                        row,
                        number,
                        categoryId,
                        isBlocked ? SeatStatus.Blocked : SeatStatus.Available
                    );

                    seats.Add(seat);
                }
            }

            _seatRepository.CreateBatch(seats);
        }

        public Seat GetSeatInfo(int seatId)
        {
            var seat = _seatRepository.GetById(seatId);
            if (seat == null)
                throw new InvalidOperationException("Место не найдено");

            return seat;
        }

        public IEnumerable<Seat> GetEventSeats(int eventId)
        {
            return _seatRepository.GetByEventId(eventId);
        }
    }
}