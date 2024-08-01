using CSharpFunctionalExtensions;
using MicroMarket.Services.Catalog.Dtos;
using MicroMarket.Services.Catalog.Models;

namespace MicroMarket.Services.Catalog.Interfaces
{
    public interface IProductsService
    {
        public Task<Result> DeleteProduct(Guid productId);
        public Task<Result<Product>> CreateProduct(ProductCreateRequestDto productCreateRequestDto);
        public Task<Result<Product>> UpdateProduct(ProductUpdateRequestDto productUpdateRequestDto);
        public Task<Result<Product>> GetProduct(Guid productId);
        public Task<Result<Product>> UpdateQuanity(Guid productId, int quantity);
        public Task<Result<Product>> DiffUpdateQuantity(Guid productId, int quantity);
    }
}
