// EventStatus - перечисление статусов события.
// Active - событие активно, билеты продаются
// Cancelled - событие отменено, все билеты аннулированы
// Completed - событие завершено, продажи билетов закрыты

namespace EventTicket.Models.Enums
{
    public enum EventStatus
    {
        Active,
        Cancelled,
        Completed
    }
}