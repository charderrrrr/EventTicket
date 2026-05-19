using System.Data;
using Dapper;
using EventTicket.Models;
using EventTicket.Models.Enums;

namespace EventTicket.Data.Repositories
{
    public class TicketRepository
    {
        private readonly IDbConnection _connection;

        public TicketRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Ticket Create(Ticket ticket)
        {
            var sql = @"INSERT INTO tickets (user_id, event_id, seat_id, purchase_date, price, status) 
                        VALUES (@UserId, @EventId, @SeatId, @PurchaseDate, @Price, @Status) RETURNING id";
            ticket.Id = _connection.QuerySingle<int>(sql, ticket);
            return ticket;
        }

        public Ticket? GetById(int id)
        {
            return _connection.QuerySingleOrDefault<Ticket>(
                @"SELECT id, user_id AS UserId, event_id AS EventId, seat_id AS SeatId, 
                         purchase_date AS PurchaseDate, price AS Price, status AS Status 
                  FROM tickets WHERE id = @Id", 
                new { Id = id });
        }

        public IEnumerable<Ticket> GetByUserId(int userId)
        {
            return _connection.Query<Ticket>(
                @"SELECT id, user_id AS UserId, event_id AS EventId, seat_id AS SeatId, 
                         purchase_date AS PurchaseDate, price AS Price, status AS Status 
                  FROM tickets WHERE user_id = @UserId ORDER BY purchase_date DESC", 
                new { UserId = userId });
        }

        public Ticket? GetBySeatId(int seatId)
        {
            return _connection.QuerySingleOrDefault<Ticket>(
                @"SELECT id, user_id AS UserId, event_id AS EventId, seat_id AS SeatId, 
                         purchase_date AS PurchaseDate, price AS Price, status AS Status 
                  FROM tickets WHERE seat_id = @SeatId AND status = @Status", 
                new { SeatId = seatId, Status = (int)TicketStatus.Active });
        }

        public void UpdateStatus(int id, TicketStatus status)
        {
            _connection.Execute(
                "UPDATE tickets SET status = @Status WHERE id = @Id", 
                new { Id = id, Status = (int)status });
        }

        public IEnumerable<Ticket> GetActiveByEventId(int eventId)
        {
            return _connection.Query<Ticket>(
                @"SELECT id, user_id AS UserId, event_id AS EventId, seat_id AS SeatId, 
                         purchase_date AS PurchaseDate, price AS Price, status AS Status 
                  FROM tickets WHERE event_id = @EventId AND status = @Status", 
                new { EventId = eventId, Status = (int)TicketStatus.Active });
        }
    }
}