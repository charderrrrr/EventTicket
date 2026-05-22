// Category - модель ценовой категории для мест в зале.
// Id - уникальный идентификатор категории
// Name - название категории (Parter, Balcony, VIP)
// BasePrice - базовая цена билета в рублях
// Multiplier - мультипликатор категории (1.0 для Партера, 0.8 для Балкона, 1.5 для VIP)
// Create - фабричный метод создания категории с указанием цены и мультипликатора

namespace EventTicket.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal Multiplier { get; set; } = 1.0m;

        public static Category Create(string name, decimal basePrice, decimal multiplier)
        {
            return new Category
            {
                Name = name,
                BasePrice = basePrice,
                Multiplier = multiplier
            };
        }
    }
}