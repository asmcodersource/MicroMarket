﻿using CSharpFunctionalExtensions;
using MicroMarket.Services.Catalog.DbContexts;
using MicroMarket.Services.Catalog.Dtos;
using MicroMarket.Services.Catalog.Interfaces;
using MicroMarket.Services.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Catalog.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly CatalogDbContext _dbContext;


        public CategoriesService(CatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // TODO: add admin options
        // TODO: add method for chaning property of active status

        public async Task<Result> DeleteCategory(Guid categoryId)
        {
            var isCategoryPresented = await _dbContext.Categories
                .Where(c => !c.IsDeleted)
                .AnyAsync(c => c.Id == categoryId);
            if (!isCategoryPresented)
                return Result.Failure($"Category {categoryId} is not exist");

            await MarkAsDeleted(categoryId);
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<Category>> GetCategory(Guid categoryId)
        {
            var category = await _dbContext.Categories
                .Where(c => !c.IsDeleted && c.IsActive)
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == categoryId);
            if (category is null)
                return Result.Failure<Category>($"Category {categoryId} is not exist");

            return Result.Success(category);
        }

        public async Task<Result<(Category, IQueryable<Product>)>> GetCategoryProducts(Guid categoryId)
        {
            var category = await _dbContext.Categories
                .Where(c => !c.IsDeleted && c.IsActive)
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == categoryId);
            if (category is null)
                return Result.Failure<(Category, IQueryable<Product>)>($"Category {categoryId} is not exist");

            var productsQuery = _dbContext.Categories
                .Where(c => c.Id == categoryId && !c.IsDeleted && c.IsActive)
                .Include(c => c.Products)
                .AsNoTracking()
                .SelectMany(c => c.Products)
                .Where(p => p.IsActive && !p.IsDeleted); // Если вам нужно фильтровать продукты
            return Result.Success((category, productsQuery));
        }

        public async Task<Result<ICollection<Category>>> GetRootCategories()
        {
            var categories = await _dbContext.Categories
                .Where(c => !c.IsDeleted && c.IsActive && c.ParentCategoryId == null)
                .Include(c => c.Products)
                .AsNoTracking()
                .ToListAsync();
            return Result.Success(categories as ICollection<Category>);
        }

        public async Task<Result<ICollection<Category>>> GetChildCategories(Guid categoryId)
        {
            var isCategoryPresented = await _dbContext.Categories.Where(c => c.Id == categoryId).AnyAsync();
            if (!isCategoryPresented)
                return Result.Failure<ICollection<Category>>($"Parent category {categoryId} is not exists");

            var categories = await _dbContext.Categories
                .Where(c => c.ParentCategoryId == categoryId)
                .AsNoTracking()
                .ToListAsync();
            return Result.Success(categories as ICollection<Category>);
        }

        public async Task<Result<Category>> UpdateCategory(CategoryUpdateRequestDto categoryUpdateRequestDto)
        {
            var category = await _dbContext.Categories
                .Where(c => !c.IsDeleted && c.IsActive)
                .Include(c => c.Products)
                .SingleOrDefaultAsync(c => c.Id == categoryUpdateRequestDto.Id);
            if (category is null)
                return Result.Failure<Category>($"Category {categoryUpdateRequestDto.Id} is not exist");
            if (categoryUpdateRequestDto.ParentCategoryId is not null)
            {
                var parentCategoryExist = await _dbContext.Categories
                    .Where(c => c.Id == categoryUpdateRequestDto.ParentCategoryId)
                    .Where(c => !c.IsDeleted)
                    .AnyAsync();
                if (!parentCategoryExist)
                    return Result.Failure<Category>($"Parent category {categoryUpdateRequestDto.ParentCategoryId} is not exist");
            }

            category.Name = categoryUpdateRequestDto.Name;
            category.Description = categoryUpdateRequestDto.Description;
            category.ParentCategoryId = categoryUpdateRequestDto.ParentCategoryId;
            await _dbContext.SaveChangesAsync();
            return Result.Success(category);
        }

        public async Task<Result<Category>> CreateCategory(CategoryCreateRequestDto categoryCreateRequestDto)
        {
            if (categoryCreateRequestDto.ParentCategoryId is not null)
            {
                var parentCategoryExist = await _dbContext.Categories
                       .Where(c => c.Id == categoryCreateRequestDto.ParentCategoryId)
                       .Where(c => !c.IsDeleted)
                       .AnyAsync();
                if (!parentCategoryExist)
                    return Result.Failure<Category>($"Parent category {categoryCreateRequestDto.ParentCategoryId} is not exist");
            }
            var createdCategory = new Category()
            {
                Name = categoryCreateRequestDto.Name,
                Description = categoryCreateRequestDto.Description,
                ParentCategoryId = categoryCreateRequestDto.ParentCategoryId
            };
            await _dbContext.Categories.AddAsync(createdCategory);
            await _dbContext.SaveChangesAsync();
            return createdCategory;
        }

        private async Task MarkAsDeleted(Guid categoryId)
        {
            var category = await _dbContext.Categories
                .Where(c => !c.IsDeleted)
                .Include(c => c.ChildCategories)
                .Include(c => c.Products)
                .FirstAsync(c => c.Id == categoryId);
            category.IsDeleted = true;
            foreach (var product in category.Products)
                product.IsDeleted = true;
            foreach (var childCategory in category.ChildCategories)
                await MarkAsDeleted(childCategory.Id);
        }

        private async Task MarkActive(Guid categoryId, bool isActive)
        {
            var category = await _dbContext.Categories
                .Where(c => !c.IsDeleted)
                .Include(c => c.ChildCategories)
                .Include(c => c.Products)
                .FirstAsync(c => c.Id == categoryId);
            category.IsActive = isActive;
            foreach (var product in category.Products)
                product.IsActive = isActive;
            foreach (var childCategory in category.ChildCategories)
                await MarkActive(childCategory.Id, isActive);
        }
    }
}
