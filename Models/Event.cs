// Event - модель события (концерт, спектакль, фильм).
// Id - уникальный идентификатор события
// Name - название мероприятия
// Date - дата и время проведения
// VenueId - идентификатор зала, где проводится событие
// Status - статус события (active, cancelled, completed)
// Venue - навигационное свойство для связи с залом
// Create - фабричный метод создания нового события

namespace EventTicket.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int VenueId { get; set; }
        public string Status { get; set; } = "active";
        public Venue? Venue { get; set; }

        public static Event Create(string name, DateTime date, int venueId)
        {
            return new Event
            {
                Name = name,
                Date = date,
                VenueId = venueId,
                Status = "active"
            };
        }
    }
}