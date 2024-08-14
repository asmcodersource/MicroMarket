using MicroMarket.Services.Catalog.Dtos;
using MicroMarket.Services.Catalog.Models;
using MicroMarket.Services.Catalog.Interfaces;
using MicroMarket.Services.SharedCore.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CSharpFunctionalExtensions;

namespace MicroMarket.Services.Catalog.Controllers
{
    [ApiController]
    [Route("api/catalog/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IManagerProductsFilterService _managerProductsFilterService;

        public ProductsController(IProductsService productsService, IManagerProductsFilterService managerProductsFilterService)
        {
            _productsService = productsService;
            _managerProductsFilterService = managerProductsFilterService;
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpGet()]
        [ProducesResponseType(typeof(Pagination<ProductGetResponseDto>.Page), 200)]
        public async Task<IActionResult> GetProducts([FromQuery] int? page, [FromQuery] int? itemsPerPage, [FromQuery] ManagerProductFilterOptions productFilterOptions)
        {
            if ((page is not null || itemsPerPage is not null) && !(page is not null && itemsPerPage is not null))
                return BadRequest("When using pagination, both parameters must be specified (page number, number of elements on the page).");
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var productsQueryResult = _productsService.GetProducts();
            if (productsQueryResult.IsFailure)
                return BadRequest(productsQueryResult.Error);
            if (page is null)
            {
                page = 0;
                itemsPerPage = int.MaxValue;
            }
            var productsQuery = productsQueryResult.Value;
            productsQuery = _managerProductsFilterService.Filter(productsQuery, productFilterOptions);
            var paginatedProducts = await Pagination<Product>.Paginate(productsQuery, page!.Value, itemsPerPage!.Value);
            if (paginatedProducts.IsFailure)
                return BadRequest(paginatedProducts.Error);
            return Ok(paginatedProducts.Value.ConvertTo(p => new ProductGetResponseDto(p)));
        }

        [AllowAnonymous, HttpGet("{productId}")]
        [ProducesResponseType(typeof(ProductGetResponseDto), 200)]
        public async Task<IActionResult> GetProduct(Guid productId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var productGetResult = await _productsService.GetProduct(productId, HasPrivilegedAccess(User));
            if (productGetResult.IsFailure)
                return BadRequest(productGetResult.Error);
            var productDto = new ProductGetResponseDto(productGetResult.Value);
            return Ok(productDto);
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpPost()]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateRequestDto productCreateRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var productCreateResult = await _productsService.CreateProduct(productCreateRequestDto);
            if (productCreateResult.IsFailure)
                return BadRequest(productCreateResult.Error);
            var productDto = new ProductGetResponseDto(productCreateResult.Value);
            return Ok(productDto);
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpPut()]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductUpdateRequestDto productUpdateRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var productUpdateResult = await _productsService.UpdateProduct(productUpdateRequestDto);
            if (productUpdateResult.IsFailure)
                return BadRequest(productUpdateResult.Error);
            var productDto = new ProductGetResponseDto(productUpdateResult.Value);
            return Ok(productDto);
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpDelete("{productId}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> RemoveProduct(Guid productId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var productDeleteResult = await _productsService.DeleteProduct(productId);
            if (productDeleteResult.IsFailure)
                return BadRequest(productDeleteResult.Error);
            return Ok();
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpPut("{productId}/constant-quantity-update")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateQuantity(Guid productId, [FromBody] int quantity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var productUpdateQuanityResult = await _productsService.UpdateQuanity(productId, quantity);
            if (productUpdateQuanityResult.IsFailure)
                return BadRequest(productUpdateQuanityResult.Error);
            var productDto = new ProductGetResponseDto(productUpdateQuanityResult.Value);
            return Ok(productDto);
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpPut("{productId}/difference-quantity-update")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DiffUpdateQuantity(Guid productId, int quantity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var productDiffUpdateResult = await _productsService.DiffUpdateQuantity(productId, quantity);
            if (productDiffUpdateResult.IsFailure)
                return BadRequest(productDiffUpdateResult.Error);
            var productDto = new ProductGetResponseDto(productDiffUpdateResult.Value);
            return Ok(productDto);
        }

        [NonAction]
        private bool HasPrivilegedAccess(ClaimsPrincipal user)
        {
            return User.Claims.Any(c => c.Type == "MANAGER" || c.Type == "ADMIN");
        }
    }
}
