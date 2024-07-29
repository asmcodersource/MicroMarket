using MicroMarket.Services.AuthAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using MicroMarket.Services.AuthAPI.Interfaces;

namespace MicroMarket.Services.AuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var userRoles = await _rolesService.AssignRoles(userId, new[] { "Admin" });
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

        [Authorize(Roles = "Admin"), HttpGet("{userId}/roles")]
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

        [Authorize(Roles = "Admin"), HttpPut("{userId}/roles")]
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

        [Authorize(Roles = "Admin"), HttpDelete("{userId}/roles")]
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
