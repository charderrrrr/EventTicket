using EventTicket.Models.Enums;

namespace EventTicket.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int SeatId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Price { get; set; }
        public TicketStatus Status { get; set; }

        public static Ticket Create(int userId, int eventId, int seatId, decimal price)
        {
            return new Ticket
            {
                UserId = userId,
                EventId = eventId,
                SeatId = seatId,
                PurchaseDate = DateTime.UtcNow,
                Price = price,
                Status = TicketStatus.Active
            };
        }
    }
}