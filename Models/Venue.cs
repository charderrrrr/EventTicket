// Venue - модель зала для проведения мероприятий.
// Id - уникальный идентификатор зала
// Name - название зала (например, "Concert Hall")
// Rows - количество рядов в зале
// SeatsPerRow - количество мест в одном ряду
// BlockedSeats - JSON-строка с координатами заблокированных мест
// GetBlockedSeats - десериализует заблокированные места из JSON
// IsSeatBlocked - проверяет, заблокировано ли конкретное место по ряду и номеру
// Create - фабричный метод создания зала с указанием заблокированных мест

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