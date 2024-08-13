using CSharpFunctionalExtensions;
using MicroMarket.Services.Ordering.Dtos;
using MicroMarket.Services.Ordering.Models;

namespace MicroMarket.Services.Ordering.Interfaces
{
    public interface IOrdersService
    {
        public Task<Result<IQueryable<Order>>> GetUserOrders(Guid initiatorUserId, Guid userId, bool onlyOwnerAllowed = true);
        public Task<Result<Order>> GetOrder(Guid initiatorUserId, Guid orderId, bool onlyOwnerAllowed = true);
        public Task<Result<Order>> UpdateOrder(Guid orderId, OrderUpdateDto orderUpdateDto);
        public Task<Result> UpdateManagerNote(Guid orderId, string note);
        public Task<Result> AddState(Guid orderId, OrderStateDto orderStateDto, bool closeOrder = false);
        public Task<Result> DeleteState(Guid stateId);
        public Task<Result> UpdateOrderDeliveryStatus(Guid orderId, bool isDelivered);
        public Task<Result> UpdateOrderPaymentStatus(Guid orderId, bool isPaid);
        public Task<Result> CloseOrder(Guid orderId);

    }
}
