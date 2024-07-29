using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MicroMarket.Services.AuthAPI.Dtos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using MicroMarket.Services.AuthAPI.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace MicroMarket.Services.AuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtProviderService;

        public AuthController(IAuthService authService, IJwtService jwtProviderService)
        {
            _authService = authService;
            _jwtProviderService = jwtProviderService;
        }

        [AllowAnonymous, HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponseDto), 200)]
        [ProducesResponseType(typeof(RegisterResponseDto), 400)]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new RegisterResponseDto(ModelState.ToList()));

            var registerResult = await _authService.Register(
                registerRequest.Name,
                registerRequest.Surname,
                registerRequest.MiddleName,
                registerRequest.Email,
                registerRequest.Password
            );
            if (registerResult.IsFailure)
                return BadRequest(new RegisterResponseDto(registerResult.Error));

            var (createdUser, createdUserRoles) = registerResult.Value;
            var jwtTokenCreateResult = await _jwtProviderService.CreateUserJwtToken(createdUser);
            if (jwtTokenCreateResult.IsFailure)
                return StatusCode(StatusCodes.Status500InternalServerError, jwtTokenCreateResult.Error);

            var encodedJwt = _jwtProviderService.EncodeToken(jwtTokenCreateResult.Value);
            var response = new RegisterResponseDto(createdUser.Id, encodedJwt, createdUserRoles);
            return Ok(response);
        }

        [AllowAnonymous, HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), 200)]
        [ProducesResponseType(typeof(LoginResponseDto), 400)]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new LoginResponseDto(ModelState.ToList()));

            var loginResult = await _authService.Login(loginRequestDto.Email, loginRequestDto.Password);
            if (loginResult.IsFailure )
                return BadRequest(new LoginResponseDto(loginResult.Error));

            var (loggedInUser, loggedInRoles) = loginResult.Value;
            var jwtTokenCreateResult = await _jwtProviderService.CreateUserJwtToken(loggedInUser);
            if( jwtTokenCreateResult.IsFailure )
                return StatusCode(StatusCodes.Status500InternalServerError, jwtTokenCreateResult.Error);

            var encodedJwt = _jwtProviderService.EncodeToken(jwtTokenCreateResult.Value);
            var response = new LoginResponseDto(loggedInUser.Id, encodedJwt, loggedInRoles);
            return Ok(response);
        }
    }
}
