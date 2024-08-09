using CSharpFunctionalExtensions;
using MicroMarket.Services.Ordering.DbContexts;
using MicroMarket.Services.Ordering.Interfaces;
using MicroMarket.Services.Ordering.Models;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using Microsoft.EntityFrameworkCore;
using MicroMarket.Services.Ordering.Dtos;
using CSharpFunctionalExtensions;

namespace MicroMarket.Services.Ordering.Services
{
    public class DraftOrdersService : IDraftOrdersService
    {
        private readonly OrderingDbContext _dbContext;

        public DraftOrdersService(OrderingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Result<Order>> ConfirmDraftOrder(Guid userId, Guid draftOrder)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<DraftOrder>> CreateDraftOrder(CreateDraftOrder createDraftOrder)
        {
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

        public async Task<Result<DraftOrder>> GetsDraftOrder(Guid userId, Guid draftOrderId)
        {
            var draftOrder = await _dbContext.DraftOrders
                .AsNoTracking()
                .Include(o => o.ClaimedItems)
                .SingleOrDefaultAsync(o => o.Id == draftOrderId);
            if (draftOrder is null)
                return Result.Failure<DraftOrder>($"Draft order {draftOrderId} is not exist");
            if (draftOrder.CustomerId != userId)
                return Result.Failure<DraftOrder>($"User {userId} haven't access to draft order {draftOrderId}");
            return Result.Success(draftOrder);
        }

        public async Task<Result<ICollection<DraftOrder>>> GetsDraftOrders(Guid userId)
        {
            var draftOrders = await _dbContext.DraftOrders
                .Where(o => o.CustomerId == userId)
                .AsNoTracking()
                .Include(o => o.ClaimedItems)
                .ToListAsync();
            return Result.Success(draftOrders as ICollection<DraftOrder>);
        }

        public Task<Result<DraftOrder>> UpdateDraftOrder(Guid userId, Guid draftOrder, DraftOrderUpdateDto draftOrderUpdateDto)
        {
            throw new NotImplementedException();
        }
    }
}
