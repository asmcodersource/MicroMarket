using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroMarket.Services.Ordering.Controllers
{
    [ApiController]
    [Route("/api/ordering/[controller]")]
    public class OrdersController: Controller
    {
        [HttpGet("my")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetMyOrders()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "ADMIN,MANAGER")]
        public async Task<IActionResult> GetOrders(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
