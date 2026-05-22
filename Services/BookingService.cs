using System.Data;
using EventTicket.Data.Repositories;
using EventTicket.Models;
using EventTicket.Models.Enums;

namespace EventTicket.Services
{
    public class BookingService
    {
        private readonly IDbConnection _connection;
        private readonly SeatRepository _seatRepository;
        private readonly TicketRepository _ticketRepository;
        private readonly PricingService _pricingService;

        public BookingService(IDbConnection connection)
        {
            _connection = connection;
            _seatRepository = new SeatRepository(connection);
            _ticketRepository = new TicketRepository(connection);
            _pricingService = new PricingService(connection);
        }

        public Ticket PurchaseTicket(int userId, int eventId, int seatId)
        {
            var seat = _seatRepository.GetById(seatId);
            if (seat == null)
                throw new InvalidOperationException("Место не найдено");

            if (seat.EventId != eventId)
                throw new InvalidOperationException("Место не принадлежит этому событию");

            if (seat.Status != SeatStatus.Available)
                throw new InvalidOperationException("Место недоступно для покупки");

            var existingTicket = _ticketRepository.GetBySeatId(seatId);
            if (existingTicket != null)
                throw new InvalidOperationException("На это место уже продан билет");

            var price = _pricingService.CalculatePrice(seatId);
            
            _seatRepository.UpdateStatus(seatId, SeatStatus.Sold);

            var ticket = Ticket.Create(userId, eventId, seatId, price);
            return _ticketRepository.Create(ticket);
        }

        public bool IsSeatAvailable(int seatId)
        {
            var seat = _seatRepository.GetById(seatId);
            if (seat == null || seat.Status != SeatStatus.Available)
                return false;
            
            var existingTicket = _ticketRepository.GetBySeatId(seatId);
            return existingTicket == null;
        }
    }
}