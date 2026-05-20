using System.Data;
using Dapper;
using Npgsql;
using EventTicket.Models;
using EventTicket.Models.Enums;

namespace EventTicket.Data
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public void Initialize()
        {
            using var connection = CreateConnection();
            
            var sql = @"
                CREATE TABLE IF NOT EXISTS venues (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(255) NOT NULL,
                    rows INT NOT NULL,
                    seats_per_row INT NOT NULL,
                    blocked_seats TEXT DEFAULT '[]'
                );

                CREATE TABLE IF NOT EXISTS categories (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(100) NOT NULL,
                    base_price DECIMAL(10,2) NOT NULL,
                    multiplier DECIMAL(5,2) DEFAULT 1.0
                );

                CREATE TABLE IF NOT EXISTS events (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(255) NOT NULL,
                    date TIMESTAMP NOT NULL,
                    venue_id INT REFERENCES venues(id),
                    status VARCHAR(50) NOT NULL DEFAULT 'active'
                );

                CREATE TABLE IF NOT EXISTS seats (
                    id SERIAL PRIMARY KEY,
                    event_id INT REFERENCES events(id) ON DELETE CASCADE,
                    row INT NOT NULL,
                    number INT NOT NULL,
                    category_id INT REFERENCES categories(id),
                    status INT NOT NULL DEFAULT 0,
                    UNIQUE(event_id, row, number)
                );

                CREATE TABLE IF NOT EXISTS tickets (
                    id SERIAL PRIMARY KEY,
                    user_id INT NOT NULL,
                    event_id INT REFERENCES events(id),
                    seat_id INT REFERENCES seats(id) UNIQUE,
                    purchase_date TIMESTAMP DEFAULT NOW(),
                    price DECIMAL(10,2) NOT NULL,
                    status INT NOT NULL DEFAULT 0
                );
            ";
            
            connection.Execute(sql);
            SeedData(connection);
        }

        private void SeedData(IDbConnection connection)
        {
            var categoryCount = connection.QuerySingle<int>("SELECT COUNT(*) FROM categories");
            if (categoryCount == 0)
            {
                connection.Execute(@"
                    INSERT INTO categories (name, base_price, multiplier) 
                    VALUES 
                    ('Parter', 1000.00, 1.0),
                    ('Balcony', 500.00, 0.8),
                    ('VIP', 3000.00, 1.5)");
            }

            var venueCount = connection.QuerySingle<int>("SELECT COUNT(*) FROM venues");
            if (venueCount == 0)
            {
                var venue = Venue.Create("Grand Hall", 5, 7, new BlockedSeat[]
                {
                    new BlockedSeat { Row = 5, Number = 7 },
                    new BlockedSeat { Row = 5, Number = 8 },
                    new BlockedSeat { Row = 6, Number = 7 },
                    new BlockedSeat { Row = 6, Number = 8 }
                });

                connection.Execute(@"
                    INSERT INTO venues (name, rows, seats_per_row, blocked_seats) 
                    VALUES (@Name, @Rows, @SeatsPerRow, @BlockedSeats)", venue);
            }

            var eventCount = connection.QuerySingle<int>("SELECT COUNT(*) FROM events");
            if (eventCount == 0)
            {
                var venueId = connection.QuerySingle<int>("SELECT id FROM venues LIMIT 1");
                var eventDate = DateTime.UtcNow.AddDays(14);
                
                connection.Execute(@"
                    INSERT INTO events (name, date, venue_id, status) 
                    VALUES (@Name, @Date, @VenueId, @Status)",
                    new { Name = "Бешеный концерт 2026", Date = eventDate, VenueId = venueId, Status = "active" });

                var eventId = connection.QuerySingle<int>("SELECT id FROM events LIMIT 1");
                var categories = connection.Query<(int Id, string Name, decimal BasePrice, decimal Multiplier)>(
                    "SELECT id, name, base_price, multiplier FROM categories").ToList();
                
                var vipId = categories.First(c => c.Name == "VIP").Id;
                var parterId = categories.First(c => c.Name == "Parter").Id;
                var balconyId = categories.First(c => c.Name == "Balcony").Id;

                var seats = new List<dynamic>();
                var blockedSeats = new[] 
                { 
                    new { Row = 5, Number = 7 }, 
                    new { Row = 4, Number = 2 }
                };

                for (int row = 1; row <= 5; row++)
                {
                    for (int number = 1; number <= 7; number++)
                    {
                        var isBlocked = blockedSeats.Any(b => b.Row == row && b.Number == number);
                        
                        int categoryId;
                        if (row <= 2)
                            categoryId = vipId;
                        else if (row <= 7)
                            categoryId = parterId;
                        else
                            categoryId = balconyId;

                        seats.Add(new
                        {
                            EventId = eventId,
                            Row = row,
                            Number = number,
                            CategoryId = categoryId,
                            Status = isBlocked ? (int)SeatStatus.Blocked : (int)SeatStatus.Available
                        });
                    }
                }

                var seatSql = @"INSERT INTO seats (event_id, row, number, category_id, status) 
                                VALUES (@EventId, @Row, @Number, @CategoryId, @Status)";
                connection.Execute(seatSql, seats);
            }
        }
    }
}