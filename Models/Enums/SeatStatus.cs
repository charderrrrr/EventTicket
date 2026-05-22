// SeatStatus - перечисление статусов места в зале.
// Available - место свободно и доступно для покупки или бронирования
// Sold - место продано, билет куплен
// Blocked - место заблокировано (сцена, оборудование, проход)

namespace EventTicket.Models.Enums
{
    public enum SeatStatus
    {
        Available,
        Sold,
        Blocked
    }
}