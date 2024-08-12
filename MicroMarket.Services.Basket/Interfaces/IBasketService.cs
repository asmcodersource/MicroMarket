using CSharpFunctionalExtensions;
using MicroMarket.Services.Basket.Models;

namespace MicroMarket.Services.Basket.Interfaces
{
    public interface IBasketService
    {
        public Task<Result<IQueryable<Item>>> GetItems(Guid initiatorUserId, Guid userId, bool onlyOwnerAllowed = true);
        public Task<Result<Item>> UpdateQuantity(Guid initiatorUserId, Guid itemId, int quantity, bool onlyOwnerAllowed = true);
        public Task<Result<Item>> AddItem(Guid initiatorUserId, Guid userId, Guid productId, int quantity, bool onlyOwnerAllowed = true);
        public Task<Result> RemoveItem(Guid initiatorUserId, Guid itemId, bool onlyOwnerAllowed = true);
        public Task<Result<Guid>> CreateOrder(Guid initiatorUserId, Guid userId, ICollection<Guid> itemsInOrder, bool onlyOwnerAllowed = true);
    }
}
