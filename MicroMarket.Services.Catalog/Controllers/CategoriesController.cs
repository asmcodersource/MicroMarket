using MicroMarket.Services.Catalog.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroMarket.Services.Catalog.Controllers
{
    [ApiController]
    [Route("api/catalog/[controller]")]
    public class CategoriesController : ControllerBase
    {
        [AllowAnonymous, HttpGet("/root")]
        public async Task<IAsyncResult> GetRootCategories()
        {
            throw new NotImplementedException();
        }

        [AllowAnonymous, HttpGet("{categoryId}")]
        public async Task<IAsyncResult> GetCategoryInfo(Guid categoryId)
        {
            throw new NotImplementedException();
        }

        [AllowAnonymous, HttpGet("{categoryId}/products")]
        public async Task<IAsyncResult> GetCategoryProducts(Guid categoryId)
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpPost]
        public async Task<IAsyncResult> CreateCategory([FromBody] CategoryCreateRequestDto? categoryCreateRequestDto)
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpDelete("{categoryId}")]
        public async Task<IAsyncResult> DeleteCategory(Guid categoryId)
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpPut("{categoryId}")]
        public async Task<IAsyncResult> UpdateCategory(Guid categoryId, [FromBody] CategoryUpdateRequestDto categoryUpdateRequestDto)
        {
            throw new NotImplementedException();
        }
    }
}
