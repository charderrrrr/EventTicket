using System;
using System.Data;
using System.Linq;
using EventTicket.Data.Repositories;
using EventTicket.Models.Enums;

namespace EventTicket.Services
{
    public class PricingService
    {
        private readonly IDbConnection _connection;
        private readonly SeatRepository _seatRepository;
        private readonly CategoryRepository _categoryRepository;

        public PricingService(IDbConnection connection)
        {
            _connection = connection;
            _seatRepository = new SeatRepository(connection);
            _categoryRepository = new CategoryRepository(connection);
        }

        public decimal CalculatePrice(int seatId)
        {
            var seat = _seatRepository.GetById(seatId);
            if (seat == null)
                throw new InvalidOperationException("Место не найдено");

            var category = _categoryRepository.GetById(seat.CategoryId);
            if (category == null)
                throw new InvalidOperationException($"Категория с id {seat.CategoryId} не найдена");

            var coefficient = CalculateDemandCoefficient(seat.EventId);
            return Math.Round(category.BasePrice * category.Multiplier * coefficient, 2);
        }

        public decimal CalculateDemandCoefficient(int eventId)
        {
            var soldSeats = _seatRepository.GetSoldSeatsCount(eventId);
            var totalSeats = _seatRepository.GetTotalAvailableSeats(eventId);

            if (totalSeats == 0)
                return 1.0m;

            var soldRatio = (decimal)soldSeats / totalSeats;

            if (soldRatio >= 0.9m) return 2.0m;
            if (soldRatio >= 0.7m) return 1.5m;
            if (soldRatio >= 0.5m) return 1.2m;
            return 1.0m;
        }

        public decimal CalculateRefundCommission(int ticketId, decimal price, DateTime eventDate)
        {
            var hoursUntilEvent = (eventDate - DateTime.UtcNow).TotalHours;

            if (hoursUntilEvent > 72) return Math.Round(price * 0.05m, 2);
            if (hoursUntilEvent > 24) return Math.Round(price * 0.15m, 2);
            if (hoursUntilEvent > 2) return Math.Round(price * 0.30m, 2);
            return Math.Round(price * 0.50m, 2);
        }
    }
}