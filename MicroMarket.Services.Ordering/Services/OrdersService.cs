using CSharpFunctionalExtensions;
using MicroMarket.Services.Ordering.Dtos;
using MicroMarket.Services.Ordering.Interfaces;
using MicroMarket.Services.Ordering.Models;

namespace MicroMarket.Services.Ordering.Services
{
    public class OrdersService : IOrdersService
    {
        public Task<Result> AddState(Guid orderId, OrderState orderState, bool closeOrder = false)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteState(Guid stateId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Order>> GetOrder(Guid initiatorUserId, Guid orderId, bool onlyOwnerAllowed = true)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IQueryable<Order>>> GetUserOrders(Guid initiatorUserId, Guid userId, bool onlyOwnerAllowed = true)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Order>> UpdateOrder(Guid orderId, OrderUpdateDto orderUpdateDto)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateOrderDeliveryStatus(Guid orderId, bool isDelivered)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateOrderPaymentStatus(Guid orderId, bool isPaid)
        {
            throw new NotImplementedException();
        }
    }
}
