namespace MicroMarket.Services.SharedCore.MessageBus.MessageContracts
{
    public class CreateDraftOrder
    {
        public record OrderItem
        {
            public Guid ProductId { get; init; }
            public string ProductName { get; init; } = string.Empty;
            public decimal ProductPrice { get; init; }
            public int ProductQuantity { get; init; }
        }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Guid CustomerId { get; set; }

    }
}
