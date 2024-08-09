using CSharpFunctionalExtensions;
using MicroMarket.Services.Basket.DbContexts;
using MicroMarket.Services.Basket.Interfaces;
using MicroMarket.Services.Basket.Models;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using MicroMarket.Services.SharedCore.RabbitMqRpc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MicroMarket.Services.Basket.Services
{
    public class BasketService : IBasketService
    {
        private readonly BasketDbContext _dbContext;
        private readonly IModel _model;
        private readonly RpcClient<AddItemToBasket, ItemInformationResponse> _itemAddRpcClient;
        private readonly RpcClient<ClaimOrderItems, ClaimedItemsResponse> _claimItemsRpcClient;
        private readonly RpcClient<CreateDraftOrder, CreatedDraftOrderResponse> _createDraftOrderRpcClient;

        public BasketService(BasketDbContext basketDbContext, BasketMessagingService basketMessagingService)
        {
            _dbContext = basketDbContext;
            _model = basketMessagingService.Model;
            _itemAddRpcClient = basketMessagingService.ItemAddRpcClient;
            _claimItemsRpcClient = basketMessagingService.ClaimItemsRpcClient;
            _createDraftOrderRpcClient = basketMessagingService.CreateDraftOrderRpcClient;
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

            if (!response.Value.IsActive)
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

        public async Task<CSharpFunctionalExtensions.Result<IQueryable<Item>>> GetItems(Guid userId, bool onlyOwnerAllowed = true)
        {
            var items = _dbContext.Items
                .Where(i => i.CustomerId == userId)
                .Include(i => i.Product)
                .AsNoTracking();
            return Result.Success<IQueryable<Item>>(items);
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

        public async Task<CSharpFunctionalExtensions.Result<Guid>> CreateOrder(Guid userId, ICollection<Guid> itemsInOrder)
        {
            var userItems = await _dbContext.Items
                .Where(i => i.CustomerId == userId && itemsInOrder.Contains(i.Id))
                .Include(i => i.Product)
                .ToListAsync();
            if( userItems.Count != itemsInOrder.Count )
                return Result.Failure<Guid>($"Not all basket objects exist");
            
            var claimRequest = new ClaimOrderItems();
            claimRequest.ItemsToClaims = userItems.Select(i => new ClaimOrderItems.ItemToClaim()
            {
                ProductId = i.Product.CatalogProductId,
                ProductQuantity = i.Quantity,
            }).ToList();
            var claimItemsResponse = await _claimItemsRpcClient.CallAsync(claimRequest);
            if( claimItemsResponse.IsFailure )
                return Result.Failure<Guid>($"Error happend in catalog service: {claimItemsResponse.Error}");
            var createDraftOrderRequest = new CreateDraftOrder()
            {
                CustomerId = userId,
                Items = claimItemsResponse.Value.ClaimedItems.Select(i =>
                    new CreateDraftOrder.OrderItem()
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        ProductPrice = i.ProductPrice,
                        ProductQuantity = i.ProductQuantity,
                    }).ToList()
            };
            var createDraftOrderResponse = await _createDraftOrderRpcClient.CallAsync(createDraftOrderRequest);
            if( createDraftOrderResponse.IsFailure)
            {
                var returnItems = new ReturnItems()
                {
                    ItemsToReturn = userItems.Select(i =>
                        new ReturnItems.ItemToReturn()
                        {
                            ProductId = i.Product.CatalogProductId,
                            ProductQuantity = i.Quantity
                        }).ToList()
                };
                var json = JsonSerializer.Serialize(returnItems);
                _model.BasicPublish("catalog.messages.exchange", "return-items", null, Encoding.UTF8.GetBytes(json));
                return CSharpFunctionalExtensions.Result.Failure<Guid>($"Some error happend during draft order creating {createDraftOrderResponse.Error}");
            } else
            {
                _dbContext.RemoveRange(userItems);
                await _dbContext.SaveChangesAsync();
                return CSharpFunctionalExtensions.Result.Success<Guid>(createDraftOrderResponse.Value.CreatedDraftOrder);
            }
        }
    }
}
