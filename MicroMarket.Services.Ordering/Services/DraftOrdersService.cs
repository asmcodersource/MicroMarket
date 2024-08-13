using CSharpFunctionalExtensions;
using MicroMarket.Services.Ordering.DbContexts;
using MicroMarket.Services.Ordering.Dtos;
using MicroMarket.Services.Ordering.Interfaces;
using MicroMarket.Services.Ordering.Models;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace MicroMarket.Services.Ordering.Services
{
    public class DraftOrdersService : IDraftOrdersService
    {
        private readonly OrderingDbContext _dbContext;
        private readonly IModel _model;
        private readonly int MaxDraftOrdersPerUser = 5;

        public DraftOrdersService(OrderingDbContext dbContext, OrderingMessagingService orderingMessagingService)
        {
            _dbContext = dbContext;
            _model = orderingMessagingService.Model;
        }

        public async Task<Result<Order>> ConfirmDraftOrder(Guid initiatorUserId, Guid draftOrderId, bool onlyOwnerAllowed = true)
        {
            var draftOrder = await _dbContext.DraftOrders
                .Where(o => o.Id == draftOrderId)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.ClaimedItems)
                .SingleOrDefaultAsync();
            if (draftOrder == null)
                return Result.Failure<Order>($"Draft order {draftOrderId} is not exist");
            if (onlyOwnerAllowed && draftOrder.CustomerId != initiatorUserId)
                return Result.Failure<Order>($"User {initiatorUserId} haven't access to draft order {draftOrderId}");
            var context = new ValidationContext(draftOrder);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(draftOrder, context, results, true))
                return Result.Failure<Order>(JsonSerializer.Serialize(results));
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                _dbContext.Remove(draftOrder);
                var order = new Order()
                {
                    CustomerId = draftOrder.CustomerId,
                    Items = draftOrder.ClaimedItems,
                    CustomerNote = draftOrder.CustomerNote,
                    DeliveryAddress = draftOrder.DeliveryAddress,
                    PaymentType = draftOrder.PaymentType,
                    CreatedAt = DateTime.UtcNow,
                    IsClosed = false,
                    IsDelivered = false,
                    IsPaid = false,
                    StateHistory = new List<OrderState>()
                };
                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return Result.Success<Order>(order);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Failure<Order>($"{ex.Message}");
            }
        }

        public async Task<Result<DraftOrder>> CreateDraftOrder(CreateDraftOrder createDraftOrder)
        {
            var userDraftOrderCount = await _dbContext.DraftOrders
                .Where(o => o.CustomerId == createDraftOrder.CustomerId)
                .CountAsync();
            if (userDraftOrderCount > MaxDraftOrdersPerUser)
                return Result.Failure<DraftOrder>($"User {createDraftOrder.CustomerId} has too many draft orders, for creating new one");
            var draftOrder = new DraftOrder()
            {
                CustomerId = createDraftOrder.CustomerId,
                ClaimedItems = createDraftOrder.Items.Select(i =>
                    new Item()
                    {
                        OrderedQuantity = i.ProductQuantity,
                        ProductName = i.ProductName,
                        ProductId = i.ProductId,
                        ProductPrice = i.ProductPrice
                    }).ToList()
            };
            _dbContext.DraftOrders.Add(draftOrder);
            await _dbContext.SaveChangesAsync();
            return Result.Success(draftOrder);
        }

        public async Task<Result> DeleteDraftOrder(Guid initiatorUserId, Guid draftOrderId, bool onlyOwnerAllowed = true)
        {
            var draftOrder = await _dbContext.DraftOrders
                .Where(o => o.Id == draftOrderId)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.ClaimedItems)
                .SingleOrDefaultAsync();
            if (draftOrder == null)
                return Result.Failure<DraftOrder>($"Draft order {draftOrderId} is not exist");
            if (onlyOwnerAllowed && draftOrder.CustomerId != initiatorUserId)
                return Result.Failure<DraftOrder>($"User {initiatorUserId} haven't access to draft order {draftOrderId}");
            _dbContext.Remove(draftOrder);
            await _dbContext.SaveChangesAsync();
            // items return
            var returnItems = new ReturnItems()
            {
                ItemsToReturn = draftOrder.ClaimedItems.Select(i =>
                    new ReturnItems.ItemToReturn()
                    {
                        ProductId = i.ProductId,
                        ProductQuantity = i.OrderedQuantity
                    }).ToList()
            };
            var json = JsonSerializer.Serialize(returnItems);
            _model.BasicPublish("catalog.messages.exchange", "return-items", null, Encoding.UTF8.GetBytes(json));
            return Result.Success();
        }

        public async Task<Result<DraftOrder>> GetDraftOrder(Guid initiatorUserId, Guid draftOrderId, bool onlyOwnerAllowed = true)
        {
            var draftOrder = await _dbContext.DraftOrders
                .AsNoTracking()
                .Include(o => o.DeliveryAddress)
                .Include(o => o.ClaimedItems)
                .SingleOrDefaultAsync(o => o.Id == draftOrderId);
            if (draftOrder is null)
                return Result.Failure<DraftOrder>($"Draft order {draftOrderId} is not exist");
            if (onlyOwnerAllowed && draftOrder.CustomerId != initiatorUserId)
                return Result.Failure<DraftOrder>($"User {initiatorUserId} haven't access to draft order {draftOrderId}");
            return Result.Success(draftOrder);
        }

        public async Task<Result<ICollection<DraftOrder>>> GetDraftOrders(Guid initiatorUserId, Guid userId, bool onlyOwnerAllowed = true)
        {
            if (onlyOwnerAllowed && initiatorUserId != userId)
                return Result.Failure<ICollection<DraftOrder>>($"User {initiatorUserId} haven't access to draft orders of user {userId}");
            var draftOrders = await _dbContext.DraftOrders
                .Where(o => o.CustomerId == userId)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.ClaimedItems)
                .AsNoTracking()
                .ToListAsync();
            return Result.Success(draftOrders as ICollection<DraftOrder>);
        }

        public async Task<Result<DraftOrder>> UpdateDraftOrder(Guid initiatorUserId, Guid draftOrderId, DraftOrderUpdateDto draftOrderUpdateDto, bool onlyOwnerAllowed = true)
        {
            var draftOrder = await _dbContext.DraftOrders
                .Where(o => o.Id == draftOrderId)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.ClaimedItems)
                .SingleOrDefaultAsync();
            if (draftOrder is null)
                return Result.Failure<DraftOrder>($"Draft order {draftOrderId} is not exist");
            if (onlyOwnerAllowed && draftOrder.CustomerId != initiatorUserId)
                return Result.Failure<DraftOrder>($"User {initiatorUserId} haven't access to draft order {draftOrderId}");
            draftOrder.DeliveryAddress = draftOrderUpdateDto.DeliveryAddress;
            draftOrder.PaymentType = draftOrderUpdateDto.PaymentType;
            draftOrder.CustomerNote = draftOrderUpdateDto.CustomerNote;
            await _dbContext.SaveChangesAsync();
            return Result.Success<DraftOrder>(draftOrder);
        }
    }
}