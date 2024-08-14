using MicroMarket.Services.Basket.Dtos;
using MicroMarket.Services.Basket.Interfaces;
using MicroMarket.Services.Basket.Models;
using MicroMarket.Services.SharedCore.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MicroMarket.Services.Basket.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpGet("{userId}/items")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        [ProducesResponseType(typeof(Pagination<BasketItemGetDto>.Page), 200)]
        public async Task<IActionResult> GetItems(Guid userId, [FromQuery] int? page, [FromQuery] int? itemsPerPage)
        {
            if ((page is not null || itemsPerPage is not null) && !(page is not null && itemsPerPage is not null))
                return BadRequest("When using pagination, both parameters must be specified (page number, number of elements on the page).");
            var initiatorUserId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var itemsGetResult = await _basketService.GetItems(initiatorUserId, userId, !HasPrivilegedAccess(User));
            if (itemsGetResult.IsFailure)
                return BadRequest(itemsGetResult.Error);
            var itemsDtoQuery = itemsGetResult.Value.Select(i => new BasketItemGetDto(i));
            if (page is null)
            {
                page = 0;
                itemsPerPage = int.MaxValue;
            }
            var paginatedItems = await Pagination<BasketItemGetDto>.Paginate(itemsDtoQuery, page!.Value, itemsPerPage!.Value);
            if (paginatedItems.IsFailure)
                return BadRequest(paginatedItems.Error);
            return Ok(paginatedItems.Value);
        }

        [HttpGet("my/items")]
        [Authorize(Roles = "CUSTOMER")]
        [ProducesResponseType(typeof(Pagination<BasketItemGetDto>.Page), 200)]
        public async Task<IActionResult> GetMyItems([FromQuery] int? page, [FromQuery] int? itemsPerPage)
        {
            if ((page is not null || itemsPerPage is not null) && !(page is not null && itemsPerPage is not null))
                return BadRequest("When using pagination, both parameters must be specified (page number, number of elements on the page).");
            var userId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var itemsGetResult = await _basketService.GetItems(userId, userId, !HasPrivilegedAccess(User));
            if (itemsGetResult.IsFailure)
                return BadRequest(itemsGetResult.Error);
            var itemsDtoQuery = itemsGetResult.Value.Select(i => new BasketItemGetDto(i));
            if (page is null)
            {
                page = 0;
                itemsPerPage = int.MaxValue;
            }
            var paginatedItems = await Pagination<BasketItemGetDto>.Paginate(itemsDtoQuery, page!.Value, itemsPerPage!.Value);
            if (paginatedItems.IsFailure)
                return BadRequest(paginatedItems.Error);
            return Ok(paginatedItems.Value);
        }

        [HttpPost("my/items/add-product/{productId}")]
        [Authorize(Roles = "CUSTOMER")]
        [ProducesResponseType(typeof(Item), 200)]
        public async Task<IActionResult> AddMyItem(Guid productId, [FromBody] BasketUpdateQuantityRequestDto newQuantityDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var userId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var initiatorUserId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var itemsAddResult = await _basketService.AddItem(initiatorUserId, userId, productId, newQuantityDto.NewQuantity, !HasPrivilegedAccess(User));
            if (itemsAddResult.IsFailure)
                return BadRequest(itemsAddResult.Error);
            return Ok(new BasketItemGetDto(itemsAddResult.Value));
        }

        [HttpPost("{userId}/items/add-product/{productId}")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        [ProducesResponseType(typeof(Item), 200)]
        public async Task<IActionResult> AddItem(Guid userId, Guid productId, [FromBody] BasketUpdateQuantityRequestDto newQuantityDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var initiatorUserId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var itemsAddResult = await _basketService.AddItem(initiatorUserId, userId, productId, newQuantityDto.NewQuantity, !HasPrivilegedAccess(User));
            if (itemsAddResult.IsFailure)
                return BadRequest(itemsAddResult.Error);
            return Ok(new BasketItemGetDto(itemsAddResult.Value));
        }

        [HttpPut("{itemId}/update-quantity")]
        [Authorize(Roles = "CUSTOMER")]
        [ProducesResponseType(typeof(Item), 200)]
        public async Task<IActionResult> UpdateQuantity(Guid itemId, [FromBody] BasketUpdateQuantityRequestDto newQuantityDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var userId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var itemsQuanitytUpdateResult = await _basketService.UpdateQuantity(userId, itemId, newQuantityDto.NewQuantity, !HasPrivilegedAccess(User));
            if (itemsQuanitytUpdateResult.IsFailure)
                return BadRequest(itemsQuanitytUpdateResult.Error);
            return Ok(new BasketItemGetDto(itemsQuanitytUpdateResult.Value));
        }

        [HttpDelete("{itemId}")]
        [Authorize(Roles = "CUSTOMER")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> RemoveItem(Guid itemId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var userId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var itemsRemoveResult = await _basketService.RemoveItem(userId, itemId, !HasPrivilegedAccess(User));
            if (itemsRemoveResult.IsFailure)
                return BadRequest(itemsRemoveResult.Error);
            return Ok();
        }

        [HttpPost("create-order")]
        [Authorize(Roles = "CUSTOMER")]
        [ProducesResponseType(typeof(Guid), 200)]
        public async Task<IActionResult> CreateOrder(ICollection<Guid> itemsInOrder)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var userId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var initiatorUserId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var orderCreateResult = await _basketService.CreateOrder(initiatorUserId, userId, itemsInOrder);
            if (orderCreateResult.IsFailure)
                return BadRequest(orderCreateResult.Error);
            return Ok(orderCreateResult.Value);
        }

        [NonAction]
        private bool HasPrivilegedAccess(ClaimsPrincipal user)
        {
            return User.Claims.Any(c => c.Type == "MANAGER" || c.Type == "ADMIN");
        }
    }
}
