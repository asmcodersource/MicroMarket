using CSharpFunctionalExtensions;
using MicroMarket.Services.Basket.DbContexts;
using MicroMarket.Services.Basket.Interfaces;
using MicroMarket.Services.Basket.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Basket.Services
{
    public class BasketService : IBasketService
    {
        private readonly BasketDbContext _dbContext; 

        public BasketService(BasketDbContext basketDbContext)
        {
            _dbContext = basketDbContext;
        }

        public Task<Result<Item>> AddItem(Guid userId, Guid productId, int quantity)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<ICollection<Item>>> GetItems(Guid userId)
        {
            var items = await _dbContext.Items.Where(i => i.CustomerId == userId).AsNoTracking().ToListAsync();
            return Result.Success<ICollection<Item>>(items);
        }

        public async Task<Result> RemoveItem(Guid itemId)
        {
            var itemToRemove = await _dbContext.Items.Where(i => i.Id == itemId).FirstOrDefaultAsync();
            if (itemToRemove is null)
                return Result.Failure<Result>($"Item {itemId} dosn't exist");
            _dbContext.Items.Remove(itemToRemove);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<Item>> UpdateQuantity(Guid itemId, int quantity)
        {
            var itemToRemove = await _dbContext.Items.Where(i => i.Id == itemId).FirstOrDefaultAsync();
            if (itemToRemove is null)
                return Result.Failure<Item>($"Item {itemId} dosn't exist");
            itemToRemove.Quantity = quantity;
            await _dbContext.SaveChangesAsync();
            return Result.Success<Item>(itemToRemove);
        }
    }
}
