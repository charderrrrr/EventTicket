using EventTicket.Models.Enums;

namespace EventTicket.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int Row { get; set; }
        public int Number { get; set; }
        public int CategoryId { get; set; }
        public SeatStatus Status { get; set; }

        public static Seat Create(int eventId, int row, int number, int categoryId, SeatStatus status)
        {
            return new Seat
            {
                EventId = eventId,
                Row = row,
                Number = number,
                CategoryId = categoryId,
                Status = status
            };
        }
    }
}