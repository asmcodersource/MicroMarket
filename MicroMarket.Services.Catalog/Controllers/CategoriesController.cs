using MicroMarket.Services.Catalog.Dtos;
using MicroMarket.Services.Catalog.Interfaces;
using MicroMarket.Services.Catalog.Models;
using MicroMarket.Services.Catalog.Services;
using MicroMarket.Services.SharedCore.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MicroMarket.Services.Catalog.Controllers
{
    [ApiController]
    [Route("api/catalog/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;
        private readonly ICustomerProductsFilterService _customerProductsFilterService;

        public CategoriesController(ICategoriesService categoriesService, ICustomerProductsFilterService customerProductsFilterService)
        {
            _categoriesService = categoriesService;
            _customerProductsFilterService = customerProductsFilterService;
        }


        [AllowAnonymous, HttpGet("root")]
        [ProducesResponseType(typeof(IEnumerable<CategoryGetResponseDto>), 200)]
        public async Task<IActionResult> GetRootCategories()
        {
            var getRootCategoryResult = await _categoriesService.GetRootCategories(HasPrivilegedAccess(User));
            if (getRootCategoryResult.IsFailure)
                return StatusCode(StatusCodes.Status500InternalServerError, getRootCategoryResult.Error);
            var categoriesDto = getRootCategoryResult.Value.Select(c => new CategoryGetResponseDto(c)).ToList();
            return Ok(categoriesDto);
        }

        [AllowAnonymous, HttpGet("{categoryId}/childs")]
        [ProducesResponseType(typeof(ICollection<Category>), 200)]
        public async Task<IActionResult> GetCategoryChilds(Guid categoryId)
        {
            var getRootCategoryResult = await _categoriesService.GetChildCategories(categoryId, HasPrivilegedAccess(User));
            if (getRootCategoryResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, getRootCategoryResult.Error);
            var categoriesDto = getRootCategoryResult.Value.Select(c => new CategoryGetResponseDto(c)).ToList();
            return Ok(categoriesDto);
        }

        [AllowAnonymous, HttpGet("{categoryId}")]
        [ProducesResponseType(typeof(CategoryGetResponseDto), 200)]
        public async Task<IActionResult> GetCategory(Guid categoryId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var getCategoryResult = await _categoriesService.GetCategory(categoryId, HasPrivilegedAccess(User));
            if (getCategoryResult.IsFailure)
                return BadRequest(getCategoryResult.Error);
            var categoryDto = new CategoryGetResponseDto(getCategoryResult.Value);
            return Ok(categoryDto);
        }

        [AllowAnonymous, HttpGet("{categoryId}/products")]
        [ProducesResponseType(typeof((CategoryGetResponseDto, Pagination<ProductGetResponseDto>.Page)), 200)]
        public async Task<IActionResult> GetCategoryProducts(Guid categoryId, [FromQuery] int? page, [FromQuery] int? itemsPerPage, [FromQuery] CustomerProductFilterOptions customerProductFilterOptions)
        {
            if ((page is not null || itemsPerPage is not null) && !(page is not null && itemsPerPage is not null))
                return BadRequest("When using pagination, both parameters must be specified (page number, number of elements on the page).");
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var getCategoryProductsResult = await _categoriesService.GetCategoryProducts(categoryId, HasPrivilegedAccess(User));
            if (getCategoryProductsResult.IsFailure)
                return BadRequest(getCategoryProductsResult.Error);
            var (category, products) = getCategoryProductsResult.Value;
            var categoryDto = new CategoryGetResponseDto(category);
            if (page is null)
            {
                page = 0;
                itemsPerPage = int.MaxValue;
            }
            var productsQuery = products;
            productsQuery = _customerProductsFilterService.Filter(productsQuery, customerProductFilterOptions);
            var paginatedProducts = await Pagination<Product>.Paginate(productsQuery, page!.Value, itemsPerPage!.Value);
            if (paginatedProducts.IsFailure)
                return BadRequest(paginatedProducts.Error);
            return Ok( new
            {
                Category = categoryDto,
                Products = paginatedProducts.Value.ConvertTo(p => new ProductGetResponseDto(p))
            });
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpPost]
        [ProducesResponseType(typeof(CategoryGetResponseDto), 200)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateRequestDto categoryCreateRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var createCategoryResult = await _categoriesService.CreateCategory(categoryCreateRequestDto);
            if (createCategoryResult.IsFailure)
                return BadRequest(createCategoryResult.Error);
            var categoryDto = new CategoryGetResponseDto(createCategoryResult.Value);
            return Ok(categoryDto);
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpDelete("{categoryId}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var deleteCategoryResult = await _categoriesService.DeleteCategory(categoryId);
            if (deleteCategoryResult.IsFailure)
                return BadRequest(deleteCategoryResult.Error);
            return Ok();
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpPut()]
        [ProducesResponseType(typeof(CategoryGetResponseDto), 200)]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateRequestDto categoryUpdateRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var updateCategoryResult = await _categoriesService.UpdateCategory(categoryUpdateRequestDto);
            if (updateCategoryResult.IsFailure)
                return BadRequest(updateCategoryResult.Error);
            var categoryDto = new CategoryGetResponseDto(updateCategoryResult.Value);
            return Ok(categoryDto);
        }

        [NonAction]
        private bool HasPrivilegedAccess(ClaimsPrincipal user)
        {
            return User.Claims.Any(c => c.Type == "MANAGER" || c.Type == "ADMIN");
        }
    }
}
