using CSharpFunctionalExtensions;
using MicroMarket.Services.Ordering.Dtos;
using MicroMarket.Services.Ordering.Models;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;

namespace MicroMarket.Services.Ordering.Interfaces
{
    public interface IDraftOrdersService
    {
        public Task<Result<ICollection<DraftOrder>>> GetDraftOrders(Guid initiatorUserId, Guid userId, bool onlyOwnerAllowed = true);
        public Task<Result<DraftOrder>> GetDraftOrder(Guid initiatorUserId, Guid draftOrderId, bool onlyOwnerAllowed = true);
        public Task<Result> DeleteDraftOrder(Guid initiatorUserId, Guid draftOrderId, bool onlyOwnerAllowed = true);
        public Task<Result<DraftOrder>> CreateDraftOrder(CreateDraftOrder createDraftOrder);
        public Task<Result<DraftOrder>> UpdateDraftOrder(Guid initiatorUserId, Guid draftOrderId, DraftOrderUpdateDto draftOrderUpdateDto, bool onlyOwnerAllowed = true);
        public Task<Result<Order>> ConfirmDraftOrder(Guid initiatorUserId, Guid draftOrderId, bool onlyOwnerAllowed = true);
    }
}
