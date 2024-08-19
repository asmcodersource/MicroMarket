using CSharpFunctionalExtensions;
using MicroMarket.Services.Catalog.Dtos;
using MicroMarket.Services.Catalog.Models;
using MicroMarket.Services.SharedCore.MessageBus.MessageContracts;

namespace MicroMarket.Services.Catalog.Interfaces
{
    public interface IProductsService
    {
        public Task<Result> ReturnItems(ReturnItems returnItems);
        public Task<Result<ClaimedItemsResponse>> ClaimItems(ClaimOrderItems claimOrderItems);
        public Result<IQueryable<Product>> GetProducts();
        public Task<Result> DeleteProduct(Guid productId);
        public Task<Result<Product>> CreateProduct(ProductCreateRequestDto productCreateRequestDto);
        public Task<Result<Product>> UpdateProduct(ProductUpdateRequestDto productUpdateRequestDto);
        public Task<Result<Product>> GetProduct(Guid productId, bool allowNonActive = false);
        public Task<Result<Product>> UpdateQuanity(Guid productId, int quantity);
        public Task<Result<Product>> DiffUpdateQuantity(Guid productId, int quantity);
    }
}
