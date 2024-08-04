using MicroMarket.Services.Basket.Models;
using CSharpFunctionalExtensions;

namespace MicroMarket.Services.Basket.Interfaces
{
    public interface IBasketService
    {
        public Task<Result<ICollection<Item>>> GetItems(Guid userId);
        public Task<Result<Item>> UpdateQuantity(Guid itemId, int quantity); 
        public Task<Result<Item>> AddItem(Guid userId, Guid productId, int quantity);
        public Task<Result> RemoveItem(Guid itemId);
    }
}
