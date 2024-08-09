using MicroMarket.Services.Ordering.Models;
using CSharpFunctionalExtensions;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using MicroMarket.Services.Ordering.Dtos;

namespace MicroMarket.Services.Ordering.Interfaces
{
    public interface IDraftOrdersService
    {
        public Task<Result<ICollection<DraftOrder>>> GetsDraftOrders(Guid userId);
        public Task<Result<DraftOrder>> GetsDraftOrder(Guid userId, Guid draftOrderId);
        public Task<Result<DraftOrder>> CreateDraftOrder(CreateDraftOrder createDraftOrder);
        public Task<Result<DraftOrder>> UpdateDraftOrder(Guid userId, Guid draftOrder, DraftOrderUpdateDto draftOrderUpdateDto);
        public Task<Result<Order>> ConfirmDraftOrder(Guid userId, Guid draftOrderId);
    }
}
