namespace MicroMarket.Services.SharedCore.MessageBus.MessageContracts
{
    public class CreatedDraftOrderResponse
    {
        public Guid CreatedDraftOrder { get; set; }

        public CreatedDraftOrderResponse() { }

        public CreatedDraftOrderResponse(Guid guid)
        {
            CreatedDraftOrder = guid;
        }
    }
}
