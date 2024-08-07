using CSharpFunctionalExtensions;
using MicroMarket.Services.Basket.DbContexts;
using MicroMarket.Services.Basket.Interfaces;
using MicroMarket.Services.Basket.Models;
using Microsoft.EntityFrameworkCore;
using MicroMarket.Services.SharedCore.MessageBus.Services;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using MicroMarket.Services.SharedCore.RabbitMqRpc;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;

namespace MicroMarket.Services.Basket.Services
{
    public class BasketService : IBasketService
    {
        private readonly BasketDbContext _dbContext;
        private readonly RpcClient<AddItemToBasket, ItemInformationResponse> _itemAddRpcClient;

        public BasketService(BasketDbContext basketDbContext, BasketMessagingService basketMessagingService)
        {
            _dbContext = basketDbContext;
            _itemAddRpcClient = basketMessagingService.ItemAddRpcClient;
        }

        public async Task<CSharpFunctionalExtensions.Result<Item>> AddItem(Guid userId, Guid productId, int quantity, bool onlyOwnerAllowed = true)
        {
            var request = new AddItemToBasket()
            {
                ItemProductId = productId,
                RequiredQuantity = quantity
            };
            var response = await _itemAddRpcClient.CallAsync(request, CancellationToken.None);
            if (response.IsFailure)
                return Result.Failure<Item>($"Item with product {productId} wasn't added to the basket, error= {response.Error}");

            var product = await _dbContext.Products.SingleOrDefaultAsync(p => p.CatalogProductId == productId);
            if (product is null)
            {
                product = new Product()
                {
                    CatalogProductId = productId,
                    Name = response.Value.ItemProductName,
                    Price = response.Value.ItemProductPrice,
                    Quantity = response.Value.AvailableQuantity,
                    IsActive = response.Value.IsActive,
                    IsDeleted = response.Value.IsDeleted
                };
                await _dbContext.Products.AddAsync(product);
            }
            else
            {
                product.CatalogProductId = productId;
                product.Quantity = response.Value.AvailableQuantity;
                product.Name = response.Value.ItemProductName;
                product.Price = response.Value.ItemProductPrice;
                product.IsActive = response.Value.IsActive;
                product.IsDeleted = response.Value.IsDeleted;
                await _dbContext.SaveChangesAsync();
            }

            if ( !response.Value.IsActive )
                return Result.Failure<Item>($"Item with product {productId} wasn't added to the basket, cause product isn't active");
            if (response.Value.IsDeleted)
                return Result.Failure<Item>($"Item with product {productId} wasn't added to the basket, cause product is deleted");
            if (response.Value.AvailableQuantity < quantity)
                return Result.Failure<Item>($"Item with product {productId} wasn't added to the basket, cause product have not enought quantity");

            var item = new Item()
            {
                CustomerId = userId,
                Quantity = quantity,
                Product = product
            };
            await _dbContext.Items.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return Result.Success<Item>(item);
        }

        public async Task<CSharpFunctionalExtensions.Result<ICollection<Item>>> GetItems(Guid userId, bool onlyOwnerAllowed = true)
        {
            var items = await _dbContext.Items
                .Where(i => i.CustomerId == userId)
                .Include(i => i.Product)
                .AsNoTracking()
                .ToListAsync();
            return Result.Success<ICollection<Item>>(items);
        }

        public async Task<Result> RemoveItem(Guid userId, Guid itemId, bool onlyOwnerAllowed = true)
        {
            var itemToRemove = await _dbContext.Items.Where(i => i.Id == itemId).FirstOrDefaultAsync();
            if (itemToRemove is null)
                return Result.Failure<Result>($"Item {itemId} dosn't exist");
            if (itemToRemove.CustomerId != userId && onlyOwnerAllowed)
                return Result.Failure<Result>($"Item {itemId} is owned by other user");
            _dbContext.Items.Remove(itemToRemove);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<CSharpFunctionalExtensions.Result<Item>> UpdateQuantity(Guid userId, Guid itemId, int quantity, bool onlyOwnerAllowed = true)
        {
            var itemToUpdate = await _dbContext.Items
                .Where(i => i.Id == itemId)
                .Include(i => i.Product)
                .FirstOrDefaultAsync();
            if (itemToUpdate is null)
                return Result.Failure<Item>($"Item {itemId} dosn't exist");
            if (itemToUpdate.CustomerId != userId && onlyOwnerAllowed)
                return Result.Failure<Item>($"Item {itemId} is owned by other user");
            itemToUpdate.Quantity = quantity;
            await _dbContext.SaveChangesAsync();
            return Result.Success<Item>(itemToUpdate);
        }
    }
}
