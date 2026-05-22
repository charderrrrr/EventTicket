// VenueRepository - репозиторий для работы с таблицей venues.
// Create - создает новый зал с указанием рядов, мест и заблокированных позиций.
// GetById - возвращает зал по идентификатору или null если не найден.
// GetAll - возвращает список всех залов, отсортированных по названию.
// Update - обновляет параметры зала (название, ряды, места, блокировки).
// Delete - физическое удаление зала из базы данных.

using System.Data;
using Dapper;
using EventTicket.Models;

namespace EventTicket.Data.Repositories
{
    public class VenueRepository
    {
        private readonly IDbConnection _connection;

        public VenueRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Venue Create(Venue venue)
        {
            var sql = @"INSERT INTO venues (name, rows, seats_per_row, blocked_seats) 
                        VALUES (@Name, @Rows, @SeatsPerRow, @BlockedSeats) RETURNING id";
            venue.Id = _connection.QuerySingle<int>(sql, venue);
            return venue;
        }

        public Venue? GetById(int id)
        {
            return _connection.QuerySingleOrDefault<Venue>(
                @"SELECT id, name AS Name, rows AS Rows, seats_per_row AS SeatsPerRow, blocked_seats AS BlockedSeats 
                  FROM venues WHERE id = @Id", 
                new { Id = id });
        }

        public IEnumerable<Venue> GetAll()
        {
            return _connection.Query<Venue>(
                @"SELECT id, name AS Name, rows AS Rows, seats_per_row AS SeatsPerRow, blocked_seats AS BlockedSeats 
                  FROM venues ORDER BY name");
        }

        public void Update(Venue venue)
        {
            _connection.Execute(@"
                UPDATE venues 
                SET name = @Name, rows = @Rows, seats_per_row = @SeatsPerRow, blocked_seats = @BlockedSeats 
                WHERE id = @Id", venue);
        }

        public void Delete(int id)
        {
            _connection.Execute("DELETE FROM venues WHERE id = @Id", new { Id = id });
        }
    }
}