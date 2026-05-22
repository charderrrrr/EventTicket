// TicketStatus - перечисление статусов купленного билета.
// Active - билет активен, место занято
// Refunded - билет возвращен, деньги возвращены с учетом комиссии
// Cancelled - билет отменен администратором или из-за отмены события

namespace EventTicket.Models.Enums
{
    public enum TicketStatus
    {
        Active,
        Refunded,
        Cancelled
    }
}