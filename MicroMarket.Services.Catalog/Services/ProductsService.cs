using CSharpFunctionalExtensions;
using MicroMarket.Services.Catalog.Enums;
using MicroMarket.Services.Catalog.DbContexts;
using MicroMarket.Services.Catalog.Dtos;
using MicroMarket.Services.Catalog.Interfaces;
using MicroMarket.Services.Catalog.Models;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace MicroMarket.Services.Catalog.Services
{
    public class ProductsService : IProductsService
    {
        private readonly CatalogDbContext _dbContext;
        private readonly RabbitMQ.Client.IModel _model;

        public ProductsService(CatalogDbContext dbContext, CatalogMessagingService catalogMessagingService)
        {
            _dbContext = dbContext;
            _model = catalogMessagingService.Model;
        }

        public async Task<Result> DeleteProduct(Guid productId)
        {
            var productToDelete = await _dbContext.Products
                .Where(p => p.Id == productId && !p.IsDeleted)
                .SingleOrDefaultAsync();
            if (productToDelete is null)
                return Result.Failure($"Product {productId} is not exist");
            productToDelete.IsDeleted = true;
            _dbContext.Products.Update(productToDelete);
            await _dbContext.SaveChangesAsync();
            ProductUpdatedEventPublish(productToDelete);
            return Result.Success();
        }

        public async Task<CSharpFunctionalExtensions.Result<Product>> CreateProduct(ProductCreateRequestDto productCreateRequestDto)
        {
            var isCatetegoryExists = await _dbContext.Categories
                .Where(c => c.Id == productCreateRequestDto.CategoryId && !c.IsDeleted)
                .AnyAsync();
            if (!isCatetegoryExists)
                return Result.Failure<Product>($"Category {productCreateRequestDto.CategoryId} is not exists");

            var createdProduct = new Product()
            {
                Name = productCreateRequestDto.Name,
                Description = productCreateRequestDto.Description,
                CategoryId = productCreateRequestDto.CategoryId,
                Price = productCreateRequestDto.Price,
                StockQuantity = productCreateRequestDto.StockQuantity,
                Weight = productCreateRequestDto.Weight,
                Dimensions = productCreateRequestDto.Dimensions,
                Brand = productCreateRequestDto.Brand,
                Manufacturer = productCreateRequestDto.Manufacturer,
                DiscountPrice = productCreateRequestDto.DiscountPrice,
                DiscountStartDate = productCreateRequestDto.DiscountStartDate,
                DiscountEndDate = productCreateRequestDto.DiscountEndDate,
            };
            await _dbContext.Products.AddAsync(createdProduct);
            await _dbContext.SaveChangesAsync();
            ProductUpdatedEventPublish(createdProduct);
            return Result.Success(createdProduct);
        }

        public async Task<CSharpFunctionalExtensions.Result<Product>> DiffUpdateQuantity(Guid productId, int quantity)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var product = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
                if (product is null)
                    throw new InvalidOperationException($"Product {productId} is not exists");

                product.StockQuantity = product.StockQuantity + quantity;
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                ProductUpdatedEventPublish(product);
                return Result.Success(product);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Failure<Product>($"Some error happend during DiffUpdateQuantity method execution, error={ex.Message}");
            }
        }

        public async Task<CSharpFunctionalExtensions.Result<Product>> GetProduct(Guid productId, bool allowNonActive = false)
        {
            var product = await _dbContext.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
            if (product is null)
                return Result.Failure<Product>($"Product {productId} is not exists");
            if( !product.IsActive && !allowNonActive )
                return Result.Failure<Product>($"Product {productId} is not active");
            return Result.Success(product);
        }

        public async Task<CSharpFunctionalExtensions.Result<Product>> UpdateProduct(ProductUpdateRequestDto productUpdateRequestDto)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var product = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == productUpdateRequestDto.Id && !p.IsDeleted);
                if (product is null)
                    throw new InvalidOperationException($"Product {productUpdateRequestDto.Id} is not exists");

                product.Name = productUpdateRequestDto.Name;
                product.Description = productUpdateRequestDto.Description;
                product.CategoryId = productUpdateRequestDto.CategoryId;
                product.Price = productUpdateRequestDto.Price;
                product.Weight = productUpdateRequestDto.Weight;
                product.Dimensions = productUpdateRequestDto.Dimensions;
                product.Brand = productUpdateRequestDto.Brand;
                product.Manufacturer = productUpdateRequestDto.Manufacturer;
                product.DiscountPrice = productUpdateRequestDto.DiscountPrice;
                product.DiscountStartDate = productUpdateRequestDto.DiscountStartDate;
                product.DiscountEndDate = productUpdateRequestDto.DiscountEndDate;
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                ProductUpdatedEventPublish(product);
                return Result.Success(product);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Failure<Product>($"Some error happend during DiffUpdateQuantity method execution, error={ex.Message}");
            }
        }

        public async Task<CSharpFunctionalExtensions.Result<Product>> UpdateQuanity(Guid productId, int quantity)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var product = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
                if (product is null)
                    throw new InvalidOperationException($"Product {productId} is not exists");

                product.StockQuantity = quantity;
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                ProductUpdatedEventPublish(product);
                return Result.Success(product);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Failure<Product>($"Some error happend during DiffUpdateQuantity method execution, error={ex.Message}");
            }
        }

        private void ProductUpdatedEventPublish(Product product)
        {
            var itemProductInfo = new ItemInformationResponse()
            {
                ItemProductId = product.Id,
                ItemProductName = product.Name,
                ItemProductPrice = product.Price,
                AvailableQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                IsDeleted = product.IsDeleted
            };
            var json = JsonSerializer.Serialize(itemProductInfo);
            var bytes = Encoding.UTF8.GetBytes(json);
            _model.BasicPublish("catalog.messages.exchange", "product-update", true, null, bytes);
        }

        public Result<IQueryable<Product>> GetProducts()
        {
            return Result.Success<IQueryable<Product>>(_dbContext.Products);
        }

        public async Task<Result<ClaimedItemsResponse>> ClaimItems(ClaimOrderItems claimOrderItems)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var operation = new ItemsClaimOperation()
                {
                    ItemsToClaim = claimOrderItems.ItemsToClaims
                };
                _dbContext.ItemsClaimOperations.Add(operation);
                await _dbContext.SaveChangesAsync();

                var outboxOperation = new OutboxOperation()
                {
                    CorrelationId = claimOrderItems.OperationId,
                    AggregationId = operation.Id,
                    ProcessedAt = DateTime.UtcNow,
                    OperationType = OutboxOperationType.ItemsClaim,
                    State = OutboxState.Completed
                };
                _dbContext.OutboxOperations.Add(outboxOperation);
                await _dbContext.SaveChangesAsync();

                var response = new ClaimedItemsResponse();
                foreach (var item in claimOrderItems.ItemsToClaims)
                {
                    var product = await _dbContext.Products
                        .Where(p => !p.IsDeleted && p.IsActive)
                        .SingleOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product is null)
                        throw new InvalidOperationException($"Product {item.ProductId} is dont exist or not active");
                    if ((product.StockQuantity - item.ProductQuantity) < 0)
                        throw new InvalidOperationException($"Not enought stock quantity of product {item.ProductId}");
                    product.StockQuantity = product.StockQuantity - item.ProductQuantity;
                    _dbContext.Update(product);
                    response.ClaimedItems.Add(
                        new ClaimedItemsResponse.ClaimedItem()
                        {
                            ProductId = item.ProductId,
                            ProductName = product.Name,
                            ProductDescription = product.Description,
                            ProductPrice = product.Price,
                            ProductQuantity = item.ProductQuantity,
                        }
                    );
                }
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return Result.Success(response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Failure<ClaimedItemsResponse>(ex.Message);
            }
        }
    }
}
