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