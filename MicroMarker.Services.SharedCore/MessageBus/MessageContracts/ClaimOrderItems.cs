namespace MicroMarket.Services.SharedCore.MessageBus.MessageContracts
{
    public class ClaimOrderItems
    {
        public record ItemToClaim
        {
            public Guid ProductId { get; init; }
            public int ProductQuantity { get; init; }
        }

        public ICollection<ItemToClaim> ItemsToClaims { get; set; } = new List<ItemToClaim>();
        public Guid OperationId { get; init; }
    }
}
