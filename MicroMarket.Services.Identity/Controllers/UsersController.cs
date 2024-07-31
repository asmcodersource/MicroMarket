using MicroMarket.Services.Identity.Dtos;
using MicroMarket.Services.Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroMarket.Services.Identity.Controllers
{
    [ApiController]
    [Route("api/identity/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IRolesService _rolesService;

        public UsersController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        // TODO: Remove this from result code
        [AllowAnonymous, HttpPut("{userId}/assing-admin")]
        [ProducesResponseType(typeof(IList<string>), 200)]
        [ProducesResponseType(typeof(IList<string>), 400)]
        public async Task<IActionResult> AssingAdminRole(string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Select(e => new string($"{e.Key} has error {e.Value}")).ToList());
            var userRoles = await _rolesService.AssignRoles(userId, new[] { "ADMIN" });
            if (userRoles.IsSuccess)
                return Ok(userRoles.Value);
            else
                return BadRequest(userRoles.Error);
        }

        [Authorize, HttpGet("my/roles")]
        [ProducesResponseType(typeof(IList<string>), 200)]
        [ProducesResponseType(typeof(IList<string>), 400)]
        public async Task<IActionResult> GetRoles()
        {
            var userId = HttpContext.User.Claims.Single(c => c.Type == "Id").Value!;
            var roles = await _rolesService.GetRoles(userId);
            if (roles.IsSuccess)
                return Ok(roles.Value);
            else
                return BadRequest(roles.Error);
        }

        [Authorize(Roles = "ADMIN"), HttpGet("{userId}/roles")]
        [ProducesResponseType(typeof(IList<string>), 200)]
        [ProducesResponseType(typeof(IList<string>), 400)]
        public async Task<IActionResult> GetRoles(string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Select(e => new string($"{e.Key} has error {e.Value}")).ToList());
            var roles = await _rolesService.GetRoles(userId);
            if (roles.IsSuccess)
                return Ok(roles.Value);
            else
                return BadRequest(roles.Error);
        }

        [Authorize(Roles = "ADMIN"), HttpPut("{userId}/roles")]
        [ProducesResponseType(typeof(IList<string>), 200)]
        [ProducesResponseType(typeof(IList<string>), 400)]
        public async Task<IActionResult> AssignRoles(string userId, [FromBody] IList<RoleRequestDto> roles)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Select(e => new string($"{e.Key} has error {e.Value}")).ToList());
            var userRoles = await _rolesService.AssignRoles(userId, roles.Select(r => r.RoleName).ToArray());
            if (userRoles.IsSuccess)
                return Ok(userRoles.Value);
            else
                return BadRequest(userRoles.Error);
        }

        [Authorize(Roles = "ADMIN"), HttpDelete("{userId}/roles")]
        [ProducesResponseType(typeof(IList<string>), 200)]
        [ProducesResponseType(typeof(IList<string>), 400)]
        public async Task<IActionResult> UnassignRoles(string userId, [FromBody] IList<RoleRequestDto> roles)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Select(e => new string($"{e.Key} has error {e.Value}")).ToList());
            var userRoles = await _rolesService.UnassignRoles(userId, roles.Select(r => r.RoleName).ToArray());
            if (userRoles.IsSuccess)
                return Ok(userRoles.Value);
            else
                return BadRequest(userRoles.Error);
        }
    }
}
