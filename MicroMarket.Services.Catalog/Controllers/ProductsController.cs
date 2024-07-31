using MicroMarket.Services.Catalog.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroMarket.Services.Catalog.Controllers
{
    [ApiController]
    [Route("api/catalog/[controller]")]
    public class ProductsController : ControllerBase
    {
        [AllowAnonymous, HttpGet("{productId}")]
        [ProducesResponseType(typeof(ProductGetResponseDto), 200)]
        public async Task<IAsyncResult> GetProduct(Guid productId)
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpPost()]
        [ProducesResponseType(200)]
        public async Task<IAsyncResult> CreateProduct([FromBody] ProductCreateRequestDto productCreateRequestDto)
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpPut("{productId}")]
        [ProducesResponseType(200)]
        public async Task<IAsyncResult> UpdateProduct(Guid productId, [FromBody] ProductUpdateRequestDto productUpdateRequestDto)
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpDelete("{productId}")]
        [ProducesResponseType(200)]
        public async Task<IAsyncResult> RemoveProduct(Guid productId)
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpPut("{productId}/constant-quantity-update")]
        [ProducesResponseType(200)]
        public async Task<IAsyncResult> UpdateQuantity(Guid productId)
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "ADMIN,MANAGER"), HttpPut("{productId}/difference-quantity-update")]
        [ProducesResponseType(200)]
        public async Task<IAsyncResult> DiffUpdateQuantity(Guid productId)
        {
            throw new NotImplementedException();
        }
    }
}
