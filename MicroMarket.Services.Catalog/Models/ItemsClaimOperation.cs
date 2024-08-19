using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;

namespace MicroMarket.Services.Catalog.Models
{
    public class ItemsClaimOperation
    {
        public Guid Id { get; set; }
        public ICollection<ClaimOrderItems.ItemToClaim> ItemsToClaim { get; set; } = null!;
    }
}
