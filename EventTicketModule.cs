using System;
using System.Data;
using EventTicket.Data;
using EventTicket.Data.Repositories;
using EventTicket.Services;

namespace EventTicket
{
    public class EventTicketModule : IDisposable
    {
        private readonly IDbConnection _connection;
        public IDbConnection Connection => _connection;
        public EventRepository EventRepository { get; }
        public VenueRepository VenueRepository { get; }
        public SeatRepository SeatRepository { get; }
        public CategoryRepository CategoryRepository { get; }
        public TicketRepository TicketRepository { get; }
        public PricingService PricingService { get; }
        public BookingService BookingService { get; }
        public RefundService RefundService { get; }
        public VenueLayoutService VenueLayoutService { get; }

        public EventTicketModule(string connectionString)
        {
            var dbService = new DatabaseService(connectionString);
            _connection = dbService.CreateConnection();

            EventRepository = new EventRepository(_connection);
            VenueRepository = new VenueRepository(_connection);
            SeatRepository = new SeatRepository(_connection);
            CategoryRepository = new CategoryRepository(_connection);
            TicketRepository = new TicketRepository(_connection);
            PricingService = new PricingService(_connection);
            BookingService = new BookingService(_connection);
            RefundService = new RefundService(_connection);
            VenueLayoutService = new VenueLayoutService(_connection);
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}