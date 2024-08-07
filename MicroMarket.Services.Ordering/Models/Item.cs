namespace MicroMarket.Services.Ordering.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int OrderedQuantity { get; set; }
        public Order Order { get; set; } = null!;
    }
}
