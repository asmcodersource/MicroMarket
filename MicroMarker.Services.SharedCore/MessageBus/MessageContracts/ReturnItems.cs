namespace MicroMarket.Services.SharedCore.MessageBus.MessageContracts
{
    public class ReturnItems
    {
        public record ItemToReturn
        {
            public Guid ProductId { get; init; }
            public int ProductQuantity { get; init; }
        }

        public ICollection<ItemToReturn> ItemsToReturn { get; set; } = new List<ItemToReturn>();
    }
}
