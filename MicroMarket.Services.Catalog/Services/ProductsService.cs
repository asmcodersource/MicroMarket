using CSharpFunctionalExtensions;
using MicroMarket.Services.Catalog.DbContexts;
using MicroMarket.Services.Catalog.Dtos;
using MicroMarket.Services.Catalog.Interfaces;
using MicroMarket.Services.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Catalog.Services
{
    public class ProductsService : IProductsService
    {
        private readonly CatalogDbContext _dbContext;

        public ProductsService(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
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
            return Result.Success();
        }

        public async Task<Result<Product>> CreateProduct(ProductCreateRequestDto productCreateRequestDto)
        {
            var isCatetegoryExists = await _dbContext.Categories
                .Where(c => c.Id == productCreateRequestDto.CategoryId && c.IsActive && !c.IsDeleted)
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
            // TODO: verify this return object with Id
            await _dbContext.Products.AddAsync(createdProduct);
            await _dbContext.SaveChangesAsync();
            return Result.Success(createdProduct);
        }

        public async Task<Result<Product>> DiffUpdateQuantity(Guid productId, int quantity)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var product = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
                if (product is null)
                    return Result.Failure<Product>($"Product {productId} is not exists");

                product.StockQuantity = product.StockQuantity + quantity;
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return Result.Success(product);
            }
            catch(Exception ex)
            {
                return Result.Failure<Product>($"Some error happend during DiffUpdateQuantity method execution, error={ex.Message}");
            }
        }

        public async Task<Result<Product>> GetProduct(Guid productId)
        {
            var product = await _dbContext.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
            if (product is null)
                return Result.Failure<Product>($"Product {productId} is not exists");
            return Result.Success(product);
        }

        public async Task<Result<Product>> UpdateProduct(ProductUpdateRequestDto productUpdateRequestDto)
        {
            var product = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == productUpdateRequestDto.Id && !p.IsDeleted);
            if (product is null)
                return Result.Failure<Product>($"Product {productUpdateRequestDto.Id} is not exists");

            product.Name = productUpdateRequestDto.Name;
            product.Description = productUpdateRequestDto.Description;
            product.CategoryId = productUpdateRequestDto.CategoryId;
            product.Price = productUpdateRequestDto.Price;
            product.StockQuantity = productUpdateRequestDto.StockQuantity;
            product.Weight = productUpdateRequestDto.Weight;
            product.Dimensions = productUpdateRequestDto.Dimensions;
            product.Brand = productUpdateRequestDto.Brand;
            product.Manufacturer = productUpdateRequestDto.Manufacturer;
            product.DiscountPrice = productUpdateRequestDto.DiscountPrice;
            product.DiscountStartDate = productUpdateRequestDto.DiscountStartDate;
            product.DiscountEndDate = productUpdateRequestDto.DiscountEndDate;
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            // TODO: verify this return object with Id
            return Result.Success(product);
        }

        public async Task<Result<Product>> UpdateQuanity(Guid productId, int quantity)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var product = await _dbContext.Products.SingleOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);
                if (product is null)
                    return Result.Failure<Product>($"Product {productId} is not exists");

                product.StockQuantity = quantity;
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return Result.Success(product);
            }
            catch (Exception ex)
            {
                return Result.Failure<Product>($"Some error happend during DiffUpdateQuantity method execution, error={ex.Message}");
            }
        }
    }
}
