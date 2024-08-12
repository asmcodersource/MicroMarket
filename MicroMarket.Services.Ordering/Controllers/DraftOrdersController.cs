using MicroMarket.Services.Ordering.Dtos;
using MicroMarket.Services.Ordering.Interfaces;
using MicroMarket.Services.Ordering.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MicroMarket.Services.Ordering.Controllers
{
    [ApiController]
    [Route("/api/ordering/draft-orders")]
    public class DraftOrdersController : ControllerBase
    {
        private readonly IDraftOrdersService _draftOrdersService;

        public DraftOrdersController(IDraftOrdersService draftOrdersService)
        {
            _draftOrdersService = draftOrdersService;
        }

        [Authorize(Roles = "CUSTOMER,ADMIN,MANAGER")]
        [HttpGet("my")]
        [ProducesResponseType(typeof(ICollection<DraftOrder>), 200)]
        public async Task<IActionResult> GetMyDraftOrders()
        {
            var userId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var getUserDraftOrdersResult = await _draftOrdersService.GetDraftOrders(userId, userId, !HasPrivilegedAccess(User));
            if (getUserDraftOrdersResult.IsFailure)
                return StatusCode(StatusCodes.Status500InternalServerError, getUserDraftOrdersResult.Error);
            return Ok(getUserDraftOrdersResult.Value);
        }

        [Authorize(Roles = "CUSTOMER,ADMIN,MANAGER")]
        [HttpGet("{draftOrderId}")]
        [ProducesResponseType(typeof(DraftOrder), 200)]
        public async Task<IActionResult> GetDraftOrder(Guid draftOrderId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var userId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var getDraftOrderResult = await _draftOrdersService.GetDraftOrder(userId, draftOrderId, !HasPrivilegedAccess(User));
            if (getDraftOrderResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, getDraftOrderResult.Error);
            return Ok(getDraftOrderResult.Value);
        }

        [Authorize(Roles = "CUSTOMER,ADMIN,MANAGER")]
        [HttpPut("{draftOrderId}")]
        [ProducesResponseType(typeof(DraftOrder), 200)]
        public async Task<IActionResult> UpdateMyDraftOrder(Guid draftOrderId, [FromBody] DraftOrderUpdateDto draftOrderUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var userId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var updateDraftOrderResult = await _draftOrdersService.UpdateDraftOrder(userId, draftOrderId, draftOrderUpdateDto, !HasPrivilegedAccess(User));
            if (updateDraftOrderResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, updateDraftOrderResult.Error);
            return Ok(updateDraftOrderResult.Value);
        }

        [Authorize(Roles = "CUSTOMER,ADMIN,MANAGER")]
        [HttpDelete("{draftOrderId}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteDraftOrder(Guid draftOrderId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var userId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var deleteDraftOrderResult = await _draftOrdersService.DeleteDraftOrder(userId, draftOrderId, !HasPrivilegedAccess(User));
            if (deleteDraftOrderResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, deleteDraftOrderResult.Error);
            return Ok();
        }

        [Authorize(Roles = "CUSTOMER,ADMIN,MANAGER")]
        [HttpPost("{draftOrderId}/confirm")]
        [ProducesResponseType(typeof(Order), 200)]
        public async Task<IActionResult> ConfirmDraftOrder(Guid draftOrderId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());
            var userId = Guid.Parse(User.Claims.First(c => c.Type == "Id").Value);
            var confirmDraftOrderResult = await _draftOrdersService.ConfirmDraftOrder(userId, draftOrderId, !HasPrivilegedAccess(User));
            if (confirmDraftOrderResult.IsFailure)
                return StatusCode(StatusCodes.Status400BadRequest, confirmDraftOrderResult.Error);
            return Ok(confirmDraftOrderResult.Value);
        }

        [NonAction]
        private bool HasPrivilegedAccess(ClaimsPrincipal user)
        {
            return User.Claims.Any(c => c.Type == "MANAGER" || c.Type == "ADMIN");
        }
    }
}
