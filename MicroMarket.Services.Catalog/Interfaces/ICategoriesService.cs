using CSharpFunctionalExtensions;
using MicroMarket.Services.Catalog.Dtos;
using MicroMarket.Services.Catalog.Models;

namespace MicroMarket.Services.Catalog.Interfaces
{
    public interface ICategoriesService
    {
        public Task<Result> DeleteCategory(Guid categoryId);
        public Task<Result<ICollection<Category>>> GetRootCategories(bool allowNonActive = false);
        public Task<Result<Category>> GetCategory(Guid categoryId, bool allowNonActive = false);
        public Task<Result<ICollection<Category>>> GetChildCategories(Guid categoryId, bool allowNonActive = false);
        public Task<Result<(Category, IQueryable<Product>)>> GetCategoryProducts(Guid categoryId, bool allowNonActive = false);
        public Task<Result<Category>> CreateCategory(CategoryCreateRequestDto categoryCreateRequestDto);
        public Task<Result<Category>> UpdateCategory(CategoryUpdateRequestDto categoryUpdateRequestDto);

    }
}
