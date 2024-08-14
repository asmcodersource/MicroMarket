using CSharpFunctionalExtensions;
using MicroMarket.Services.Ordering.DbContexts;
using MicroMarket.Services.Ordering.Dtos;
using MicroMarket.Services.Ordering.Interfaces;
using MicroMarket.Services.Ordering.Models;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace MicroMarket.Services.Ordering.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly OrderingDbContext _dbContext;
        private readonly OrderingMessagingService _orderingMessagingService;

        public OrdersService(OrderingDbContext orderingDbContext, OrderingMessagingService orderingMessagingService)
        {
            _dbContext = orderingDbContext;
            _orderingMessagingService = orderingMessagingService;
        }

        public async Task<Result> AddState(Guid orderId, OrderStateDto orderStateDto, bool closeOrder = false)
        {
            var order = await _dbContext.Orders
                .Include(o => o.StateHistory)
                .SingleOrDefaultAsync(o => o.Id == orderId);
            if (order is null)
                return Result.Failure($"Order {orderId} is not exist");
            var orderState = new OrderState()
            {
                Name = orderStateDto.Name,
                Description = orderStateDto.Description,
                Type = orderStateDto.Type,
                OccuredAt = DateTime.UtcNow
            };
            order.StateHistory.Add(orderState);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteState(Guid stateId)
        {
            var state = await _dbContext.OrderStates.SingleOrDefaultAsync(s => s.Id == stateId);
            if (state is null)
                return Result.Failure($"State {stateId} is not exist");
            _dbContext.OrderStates.Remove(state);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<Order>> GetOrder(Guid initiatorUserId, Guid orderId, bool onlyOwnerAllowed = true)
        {
            var order = await _dbContext.Orders
                .Include(o => o.StateHistory)
                .Include(o => o.DeliveryAddress)
                .AsNoTracking()
                .SingleOrDefaultAsync(o => o.Id == orderId);
            if (order is null)
                return Result.Failure<Order>($"Order {orderId} is not exist");
            if( onlyOwnerAllowed && initiatorUserId != order.CustomerId )
                return Result.Failure<Order>($"User {initiatorUserId} haven't access to order {orderId}");
            return Result.Success(order);
        }

        public async Task<Result<IQueryable<Order>>> GetUserOrders(Guid initiatorUserId, Guid userId, bool onlyOwnerAllowed = true)
        {
            if( onlyOwnerAllowed && initiatorUserId != userId )
                return Result.Failure<IQueryable<Order>>($"User {initiatorUserId} haven't access to draft orders of user {userId}");
            var ordersQuery = _dbContext.Orders
                .Include(o => o.StateHistory)
                .Include(o => o.DeliveryAddress)
                .Where(o => o.CustomerId == userId);
            return Result.Success(ordersQuery);
        }

        public async Task<Result<Order>> UpdateOrder(Guid orderId, OrderUpdateDto orderUpdateDto)
        {
            var order = await _dbContext.Orders
               .Include(o => o.DeliveryAddress)
               .SingleOrDefaultAsync(o => o.Id == orderId);
            if (order is null)
                return Result.Failure<Order>($"Order {orderId} is not exist");
            order.DeliveryAddress = orderUpdateDto.DeliveryAddress;
            order.IsClosed = orderUpdateDto.IsClosed;
            order.IsDelivered = orderUpdateDto.IsDelivered;
            order.IsPaid = orderUpdateDto.IsPaid;
            await _dbContext.SaveChangesAsync();
            return Result.Success(order);
        }

        public async Task<Result> UpdateOrderDeliveryStatus(Guid orderId, bool isDelivered)
        {
            var order = await _dbContext.Orders
              .SingleOrDefaultAsync(o => o.Id == orderId);
            if (order is null)
                return Result.Failure<Order>($"Order {orderId} is not exist");
            order.IsDelivered = isDelivered;
            // Can do: send message to notifications service
            return Result.Success();
        }

        public async Task<Result> UpdateOrderPaymentStatus(Guid orderId, bool isPaid)
        {
            var order = await _dbContext.Orders
              .SingleOrDefaultAsync(o => o.Id == orderId);
            if (order is null)
                return Result.Failure<Order>($"Order {orderId} is not exist");
            order.IsPaid = isPaid;
            // Can do: send message to notifications service
            return Result.Success();
        }

        public async Task<Result> CloseOrder(Guid orderId)
        {
            var order = await _dbContext.Orders
                .Include(o => o.Items)
                .SingleOrDefaultAsync(o => o.Id == orderId);
            if (order is null)
                return Result.Failure($"Order {orderId} is not exist");
            if(!order.IsDelivered)
            {
                // return items to catalog
                var returnItems = new ReturnItems()
                {
                    ItemsToReturn = order.Items.Select(i =>
                        new ReturnItems.ItemToReturn()
                        {
                            ProductId = i.ProductId,
                            ProductQuantity = i.OrderedQuantity
                        }).ToList()
                };
                _orderingMessagingService.ReturnItemsToCatalog(returnItems);
            }
            order.IsClosed = true;
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> UpdateManagerNote(Guid orderId, string note)
        {
            var order = await _dbContext.Orders
                .SingleOrDefaultAsync(o => o.Id == orderId);
            if (order is null)
                return Result.Failure($"Order {orderId} is not exist");
            order.ManagerNote = note;
            return Result.Success();
        }

        public async Task<Result<IQueryable<Order>>> GetOrders()
        {
            return Result.Success<IQueryable<Order>>(_dbContext.Orders.AsQueryable());
        }
    }
}
