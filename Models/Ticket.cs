// Ticket - модель купленного билета на событие.
// Id - уникальный идентификатор билета
// UserId - идентификатор пользователя, купившего билет
// EventId - идентификатор события, на которое куплен билет
// SeatId - идентификатор конкретного места
// PurchaseDate - дата и время покупки билета
// Price - финальная цена билета с учетом динамического коэффициента
// Status - статус билета (Active, Refunded, Cancelled)
// Create - фабричный метод создания билета с текущей датой покупки

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