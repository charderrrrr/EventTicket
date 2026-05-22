// Seat - модель места в зале для конкретного события.
// Id - уникальный идентификатор места
// EventId - идентификатор события, к которому привязано место
// Row - ряд, в котором находится место
// Number - номер места в ряду
// CategoryId - идентификатор ценовой категории (VIP, Партер, Балкон)
// Status - статус места (Available, Sold, Blocked)
// Create - фабричный метод создания места с указанием статуса

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