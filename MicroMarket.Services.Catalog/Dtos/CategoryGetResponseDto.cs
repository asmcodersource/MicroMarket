using MicroMarket.Services.Catalog.Models;

namespace MicroMarket.Services.Catalog.Dtos
{
    public class CategoryGetResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Category? ParentCategory { get; set; } = null;
        public ICollection<Category> ChildCategories { get; set; }
        public bool IsActive { get; set; } = false;

        public CategoryGetResponseDto(Category category)
        {
            Id = category.Id;
            Name = category.Name;
            Description = category.Description;
            ParentCategory = category.ParentCategory;
            ChildCategories = category.ChildCategories;
            IsActive = category.IsActive;
        }
    }
}
