using CSharpFunctionalExtensions;
using MicroMarket.Services.Basket.Models;

namespace MicroMarket.Services.Basket.Interfaces
{
    public interface IBasketService
    {
        public Task<Result<IQueryable<Item>>> GetItems(Guid userId, bool onlyOwnerAllowed = true);
        public Task<Result<Item>> UpdateQuantity(Guid userId, Guid itemId, int quantity, bool onlyOwnerAllowed = true);
        public Task<Result<Item>> AddItem(Guid userId, Guid productId, int quantity, bool onlyOwnerAllowed = true);
        public Task<Result> RemoveItem(Guid userId, Guid itemId, bool onlyOwnerAllowed = true);
        public Task<Result<Guid>> CreateOrder(Guid userId, ICollection<Guid> itemsInOrder); 
    }
}
