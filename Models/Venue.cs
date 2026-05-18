using System.Text.Json;

namespace EventTicket.Models
{
    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Rows { get; set; }
        public int SeatsPerRow { get; set; }
        public string BlockedSeats { get; set; } = "[]";

        public static Venue Create(string name, int rows, int seatsPerRow, BlockedSeat[]? blockedSeats = null)
        {
            return new Venue
            {
                Name = name,
                Rows = rows,
                SeatsPerRow = seatsPerRow,
                BlockedSeats = JsonSerializer.Serialize(blockedSeats ?? Array.Empty<BlockedSeat>())
            };
        }

        public BlockedSeat[] GetBlockedSeats()
        {
            return JsonSerializer.Deserialize<BlockedSeat[]>(BlockedSeats) ?? Array.Empty<BlockedSeat>();
        }

        public bool IsSeatBlocked(int row, int number)
        {
            var blocked = GetBlockedSeats();
            return blocked.Any(b => b.Row == row && b.Number == number);
        }
    }

    public class BlockedSeat
    {
        public int Row { get; set; }
        public int Number { get; set; }
    }
}