using MicroMarket.Services.Catalog.Dtos;
using MicroMarket.Services.Catalog.Interfaces;
using MicroMarket.Services.Catalog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroMarket.Services.Catalog.Controllers
{
    [ApiController]
    [Route("api/catalog/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }


        [AllowAnonymous, HttpGet("{productId}")]
        [ProducesResponseType(typeof(ProductGetResponseDto), 200)]
        public async Task<IActionResult> GetProduct(Guid productId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var productGetResult = await _productsService.GetProduct(productId);
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
    }
}
