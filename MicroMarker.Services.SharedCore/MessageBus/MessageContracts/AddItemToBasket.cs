namespace MicroMarket.Services.SharedCore.MessageBus.MessageContracts
{
    public class AddItemToBasket
    {
        public Guid ItemProductId { get; set; }
        public int RequiredQuantity { get; set; }
    }
}
