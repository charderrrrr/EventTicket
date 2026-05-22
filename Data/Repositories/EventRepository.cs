// EventRepository - репозиторий для работы с таблицей events.
// Create - создает новое событие и возвращает его с присвоенным Id.
// GetById - возвращает событие по идентификатору или null если не найдено.
// GetAll - возвращает список всех активных событий, отсортированных по дате.
// Update - обновляет название, дату, зал и статус события.
// Delete - мягкое удаление: меняет статус события на "cancelled".

using System.Data;
using Dapper;
using EventTicket.Models;

namespace EventTicket.Data.Repositories
{
    public class EventRepository
    {
        private readonly IDbConnection _connection;

        public EventRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Event Create(Event evt)
        {
            var sql = @"INSERT INTO events (name, date, venue_id, status) 
                        VALUES (@Name, @Date, @VenueId, @Status) RETURNING id";
            evt.Id = _connection.QuerySingle<int>(sql, evt);
            return evt;
        }

        public Event? GetById(int id)
        {
            return _connection.QuerySingleOrDefault<Event>(
                @"SELECT id, name AS Name, date AS Date, venue_id AS VenueId, status AS Status 
                  FROM events WHERE id = @Id", 
                new { Id = id });
        }

        public IEnumerable<Event> GetAll()
        {
            return _connection.Query<Event>(
                @"SELECT id, name AS Name, date AS Date, venue_id AS VenueId, status AS Status 
                  FROM events WHERE status = 'active' ORDER BY date");
        }

        public void Update(Event evt)
        {
            _connection.Execute(@"
                UPDATE events 
                SET name = @Name, date = @Date, venue_id = @VenueId, status = @Status 
                WHERE id = @Id", evt);
        }

        public void Delete(int id)
        {
            _connection.Execute(
                "UPDATE events SET status = 'cancelled' WHERE id = @Id", new { Id = id });
        }
    }
}