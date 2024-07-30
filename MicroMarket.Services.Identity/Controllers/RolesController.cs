using MicroMarket.Services.Identity.Dtos;
using MicroMarket.Services.Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroMarket.Services.Identity.Controllers
{
    [ApiController]
    [Route("api/identity/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [Authorize(Roles = "Admin"), HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(IList<string>), 400)]
        public async Task<IActionResult> AddRole([FromBody] RoleRequestDto role)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Select(e => new string($"{e.Key} has error {e.Value}")).ToList());
            var result = await _rolesService.AddRole(role.RoleName);
            if (result.IsSuccess)
                return Ok();
            else
                return BadRequest(result.Error);
        }

        [Authorize(Roles = "Admin"), HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(IList<string>), 400)]
        public async Task<IActionResult> RemoveRole([FromBody] RoleRequestDto role)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Select(e => new string($"{e.Key} has error {e.Value}")).ToList());
            var result = await _rolesService.RemoveRole(role.RoleName);
            if (result.IsSuccess)
                return Ok();
            else
                return BadRequest(result.Error);
        }

        [Authorize(Roles = "Admin"), HttpGet]
        [ProducesResponseType(typeof(IList<string>), 200)]
        [ProducesResponseType(typeof(IList<string>), 400)]
        public async Task<IActionResult> GetExistRoles()
        {
            var result = await _rolesService.GetExistRoles();
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [Authorize(Roles = "Admin"), HttpGet("{role}/users")]
        [ProducesResponseType(typeof(List<UserResponseDto>), 200)]
        [ProducesResponseType(typeof(IList<string>), 400)]
        public async Task<IActionResult> GetRoleUsers(string role)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Select(e => new string($"{e.Key} has error {e.Value}")).ToList());
            var result = await _rolesService.GetRoleUsers(role);
            if (result.IsSuccess)
                return Ok(result.Value.Select(u => new UserResponseDto(u.Id, u.Name, u.Surname, u.MiddleName, u.Email)).ToList());
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
