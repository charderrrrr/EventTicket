// SeatRepository - репозиторий для работы с таблицей seats.
// Create - создает одно место с привязкой к событию и категории.
// CreateBatch - массовое создание мест в одной транзакции для генерации схемы зала.
// GetById - возвращает место по идентификатору или null.
// GetByEventId - возвращает все места конкретного события, отсортированные по ряду и номеру.
// UpdateStatus - изменяет статус места (Available, Sold, Blocked).
// GetSoldSeatsCount - возвращает количество проданных мест для расчета коэффициента спроса.
// GetTotalAvailableSeats - возвращает общее количество доступных мест (исключая заблокированные).

using System.Data;
using Dapper;
using EventTicket.Models;
using EventTicket.Models.Enums;

namespace EventTicket.Data.Repositories
{
    public class SeatRepository
    {
        private readonly IDbConnection _connection;

        public SeatRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Create(Seat seat)
        {
            var sql = @"INSERT INTO seats (event_id, row, number, category_id, status) 
                        VALUES (@EventId, @Row, @Number, @CategoryId, @Status) RETURNING id";
            seat.Id = _connection.QuerySingle<int>(sql, seat);
        }

        public void CreateBatch(IEnumerable<Seat> seats)
        {
            using var transaction = _connection.BeginTransaction();
            
            var sql = @"INSERT INTO seats (event_id, row, number, category_id, status) 
                        VALUES (@EventId, @Row, @Number, @CategoryId, @Status)";
            _connection.Execute(sql, seats, transaction);
            
            transaction.Commit();
        }

        public Seat? GetById(int id)
        {
            return _connection.QuerySingleOrDefault<Seat>(
                @"SELECT id, event_id AS EventId, row AS Row, number AS Number, 
                         category_id AS CategoryId, status AS Status 
                  FROM seats WHERE id = @Id", 
                new { Id = id });
        }

        public IEnumerable<Seat> GetByEventId(int eventId)
        {
            return _connection.Query<Seat>(
                @"SELECT id, event_id AS EventId, row AS Row, number AS Number, 
                         category_id AS CategoryId, status AS Status 
                  FROM seats WHERE event_id = @EventId ORDER BY row, number", 
                new { EventId = eventId });
        }

        public void UpdateStatus(int id, SeatStatus status)
        {
            _connection.Execute(
                "UPDATE seats SET status = @Status WHERE id = @Id", 
                new { Id = id, Status = (int)status });
        }

        public int GetSoldSeatsCount(int eventId)
        {
            return _connection.QuerySingle<int>(
                "SELECT COUNT(*) FROM seats WHERE event_id = @EventId AND status = @Status", 
                new { EventId = eventId, Status = (int)SeatStatus.Sold });
        }

        public int GetTotalAvailableSeats(int eventId)
        {
            return _connection.QuerySingle<int>(
                "SELECT COUNT(*) FROM seats WHERE event_id = @EventId AND status != @Status", 
                new { EventId = eventId, Status = (int)SeatStatus.Blocked });
        }
    }
}