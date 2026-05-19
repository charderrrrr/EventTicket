using System;
using System.Data;
using EventTicket.Data.Repositories;
using EventTicket.Models.Enums;

namespace EventTicket.Services
{
    public class RefundService
    {
        private readonly IDbConnection _connection;
        private readonly TicketRepository _ticketRepository;
        private readonly SeatRepository _seatRepository;
        private readonly EventRepository _eventRepository;
        private readonly PricingService _pricingService;

        public RefundService(IDbConnection connection)
        {
            _connection = connection;
            _ticketRepository = new TicketRepository(connection);
            _seatRepository = new SeatRepository(connection);
            _eventRepository = new EventRepository(connection);
            _pricingService = new PricingService(connection);
        }

        public RefundResult RefundTicket(int ticketId)
        {
            var ticket = _ticketRepository.GetById(ticketId);
            if (ticket == null)
                throw new InvalidOperationException("Билет не найден");

            if (ticket.Status != TicketStatus.Active)
                throw new InvalidOperationException("Билет уже возвращен или отменен");

            var evt = _eventRepository.GetById(ticket.EventId);
            if (evt == null)
                throw new InvalidOperationException("Событие не найдено");

            var commission = _pricingService.CalculateRefundCommission(ticketId, ticket.Price, evt.Date);
            var refundAmount = ticket.Price - commission;

            _ticketRepository.UpdateStatus(ticketId, TicketStatus.Refunded);
            _seatRepository.UpdateStatus(ticket.SeatId, SeatStatus.Available);

            return new RefundResult
            {
                TicketId = ticketId,
                OriginalPrice = ticket.Price,
                Commission = commission,
                RefundAmount = refundAmount
            };
        }
    }

    public class RefundResult
    {
        public int TicketId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal Commission { get; set; }
        public decimal RefundAmount { get; set; }
    }
}