using MicroMarket.Services.Basket.Models;
using CSharpFunctionalExtensions;

namespace MicroMarket.Services.Basket.Interfaces
{
    public interface IBasketService
    {
        public Task<Result<ICollection<Item>>> GetItems(Guid userId, bool onlyOwnerAllowed = true);
        public Task<Result<Item>> UpdateQuantity(Guid userId, Guid itemId, int quantity, bool onlyOwnerAllowed = true); 
        public Task<Result<Item>> AddItem(Guid userId, Guid productId, int quantity, bool onlyOwnerAllowed = true);
        public Task<Result> RemoveItem(Guid userId, Guid itemId, bool onlyOwnerAllowed = true);
    }
}
