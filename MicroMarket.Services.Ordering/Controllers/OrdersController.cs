using MicroMarket.Services.Ordering.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroMarket.Services.Ordering.Dtos;
using MicroMarket.Services.Ordering.Interfaces;
using MicroMarket.Services.Ordering.Models;
using MicroMarket.Services.Ordering.Services;
using StackExchange.Redis;
using System.Security.Claims;
using MicroMarket.Services.SharedCore.Pagination;

namespace MicroMarket.Services.Ordering.Controllers
{
    [ApiController]
    [Route("/api/ordering/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IManagerFilterService _managerFilterService;
        private readonly ICustomerFilterService _customerFilterService;
        private readonly IOrdersService _orderingService;

        public OrdersController(IOrdersService ordersService, IManagerFilterService managerFilterService, ICustomerFilterService customerFilterService)
        {
            _orderingService = ordersService;
            _managerFilterService = managerFilterService;
            _customerFilterService = customerFilterService;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,MANAGER")]
        [ProducesResponseType(typeof(Pagination<Models.Order>.Page), 200)]
        public async Task<IActionResult> GetOrders([FromQuery] int? page, [FromQuery] int? itemsPerPage, [FromQuery] ManagerFilterOptions managerFilterOptions)
        {
            if ((page is not null || itemsPerPage is not null) && !(page is not null && itemsPerPage is not null))
                return BadRequest("When using pagination, both parameters must be specified (page number, number of elements on the page).");
            var getUserOrdersResult = await _orderingService.GetOrders();
            if (getUserOrdersResult.IsFailure)
                return StatusCode(StatusCodes.Status500InternalServerError, getUserOrdersResult.Error);
            if (page is null)
            {
                page = 0;
                itemsPerPage = int.MaxValue;
            }
            var ordersQuery = getUserOrdersResult.Value;
            ordersQuery = _managerFilterService.Filter(ordersQuery, managerFilterOptions);
            var paginationResult = await Pagination<Models.Order>.Paginate(ordersQuery, page.Value!, itemsPerPage!.Value);
            if (paginationResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, paginationResult.Error);
            return Ok(paginationResult.Value);
        }

        [HttpGet("my")]
        [Authorize(Roles = "CUSTOMER")]
        [ProducesResponseType(typeof(Pagination<Models.Order>.Page), 200)]
        public async Task<IActionResult> GetMyOrders([FromQuery] int? page, [FromQuery] int? itemsPerPage, [FromQuery] CustomerFilterOptions customerFilterOptions)
        {
            if ((page is not null || itemsPerPage is not null) && !(page is not null && itemsPerPage is not null))
                return BadRequest("When using pagination, both parameters must be specified (page number, number of elements on the page).");
            var userId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var getUserOrdersResult = await _orderingService.GetUserOrders(userId, userId);
            if (getUserOrdersResult.IsFailure)
                return StatusCode(StatusCodes.Status500InternalServerError, getUserOrdersResult.Error);
            if (page is null)
            {
                page = 0;
                itemsPerPage = int.MaxValue;
            }
            var ordersQuery = getUserOrdersResult.Value;
            ordersQuery = _customerFilterService.Filter(ordersQuery, customerFilterOptions);
            var paginationResult = await Pagination<Models.Order>.Paginate(ordersQuery, page.Value!, itemsPerPage!.Value);
            if (paginationResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, paginationResult.Error);
            return Ok(paginationResult.Value);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "CUSTOMER,ADMIN,MANAGER")]
        [ProducesResponseType(typeof(Pagination<Models.Order>.Page), 200)]
        public async Task<IActionResult> GetUserOrders(Guid userId, [FromQuery] int? page, [FromQuery] int? itemsPerPage, [FromQuery] CustomerFilterOptions customerFilterOptions)
        {
            if ((page is not null || itemsPerPage is not null) && !(page is not null && itemsPerPage is not null))
                return BadRequest("When using pagination, both parameters must be specified (page number, number of elements on the page).");
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var initiatorUserId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var getUserOrdersResult = await _orderingService.GetUserOrders(initiatorUserId, userId, !HasPrivilegedAccess(User));
            if (getUserOrdersResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, getUserOrdersResult.Error);
            if (page is null)
            {
                page = 0;
                itemsPerPage = int.MaxValue;
            }
            var ordersQuery = getUserOrdersResult.Value;
            ordersQuery = _customerFilterService.Filter(ordersQuery, customerFilterOptions);
            var paginationResult = await Pagination<Models.Order>.Paginate(ordersQuery, page.Value!, itemsPerPage!.Value);
            if (paginationResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, paginationResult.Error);
            return Ok(paginationResult.Value);
        }

        [HttpGet("{orderId}")]
        [Authorize(Roles = "CUSTOMER,ADMIN,MANAGER")]
        [ProducesResponseType(typeof(Models.Order), 200)]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var initiatorUserId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var getOrderResult = await _orderingService.GetOrder(initiatorUserId, orderId, !HasPrivilegedAccess(User));
            if (getOrderResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, getOrderResult.Error);
            return Ok(getOrderResult.Value);
        }

        [HttpPut("{orderId}")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        [ProducesResponseType(typeof(Models.Order), 200)]
        public async Task<IActionResult> UpdateOrder(Guid orderId, [FromBody] OrderUpdateDto orderUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var updateOrderResult = await _orderingService.UpdateOrder(orderId, orderUpdateDto);
            if (updateOrderResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, updateOrderResult.Error);
            return Ok(updateOrderResult.Value);
        }

        [HttpPost("{orderId}/add-state")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddOrderState(Guid orderId, [FromBody] OrderStateDto newOrderState )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var addOrderStateResult = await _orderingService.AddState(orderId, newOrderState);
            if (addOrderStateResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, addOrderStateResult.Error);
            return Ok();
        }

        [HttpDelete("{orderStateId}/delete-state")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteOrderState(Guid orderStateId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var deleteOrderStateResult = await _orderingService.DeleteState(orderStateId);
            if (deleteOrderStateResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, deleteOrderStateResult.Error);
            return Ok();
        }

        [HttpPut("{orderId}/delivery-state")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateOrderDeliveryStatus(Guid orderId, bool isDelivered)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var updateDeliveryStatusResult = await _orderingService.UpdateOrderDeliveryStatus(orderId, isDelivered);
            if (updateDeliveryStatusResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, updateDeliveryStatusResult.Error);
            return Ok();
        }

        [HttpPut("{orderId}/payment-state")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateOrderPaymentStatus(Guid orderId, bool isPaid)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var updatePaymentStatusResult = await _orderingService.UpdateOrderDeliveryStatus(orderId, isPaid);
            if (updatePaymentStatusResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, updatePaymentStatusResult.Error);
            return Ok();
        }

        [HttpPut("{orderId}/manager-note")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateManagerNote(Guid orderId, string managerNote)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var updateManagerNoteResult = await _orderingService.UpdateManagerNote(orderId, managerNote);
            if (updateManagerNoteResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, updateManagerNoteResult.Error);
            return Ok();
        }

        [HttpPost("{orderId}/close-order")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CloseOrder(Guid orderId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var closeOrderResult = await _orderingService.CloseOrder(orderId);
            if (closeOrderResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, closeOrderResult.Error);
            return Ok();
        }

        [NonAction]
        private bool HasPrivilegedAccess(ClaimsPrincipal user)
        {
            return User.Claims.Any(c => c.Type == "MANAGER" || c.Type == "ADMIN");
        }
    }
}
