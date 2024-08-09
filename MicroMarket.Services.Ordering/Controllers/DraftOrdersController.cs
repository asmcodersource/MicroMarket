using MicroMarket.Services.Ordering.Dtos;
using MicroMarket.Services.Ordering.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroMarket.Services.Ordering.Controllers
{
    [ApiController]
    [Route("/api/ordering/[controller]")]
    public class DraftOrdersController
    {
        private readonly IDraftOrdersService _draftOrdersService;
        
        public DraftOrdersController(IDraftOrdersService draftOrdersService)
        {
            _draftOrdersService = draftOrdersService;
        }

        [Authorize(Roles = "CUSTOMER,ADMIN,MANAGER")]
        [HttpGet("my-draft-orders")]
        public async Task<IActionResult> GetMyDraftOrders()
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "CUSTOMER,ADMIN,MANAGER")]
        [HttpGet("draft-order/{draftOrderId}")]
        public async Task<IActionResult> GetDraftOrder(Guid draftOrderId)
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "CUSTOMER,ADMIN,MANAGER")]
        [HttpPut("draft-order/{draftOrderId}")]
        public async Task<IActionResult> UpdateMyDraftOrder(Guid draftOrderId, [FromBody] DraftOrderUpdateDto draftOrderUpdateDto)
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "CUSTOMER,ADMIN,MANAGER")]
        [HttpDelete("draft-order/{draftOrderId}")]
        public async Task<IActionResult> DeleteDraftOrder(Guid draftOrderId)
        {
            throw new NotImplementedException();
        }
    }
}
