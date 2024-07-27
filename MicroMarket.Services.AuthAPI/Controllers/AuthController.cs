﻿using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MicroMarket.Services.AuthAPI.Dtos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using MicroMarket.Services.AuthAPI.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace MicroMarket.Services.AuthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: Controller
    {
        private readonly IAuthService _authService;
        private readonly IJwtProviderService _jwtProviderService;

        public AuthController(IAuthService authService, IJwtProviderService jwtProviderService)
        {
            _authService = authService;
            _jwtProviderService = jwtProviderService;
        }

        [HttpPost("register")]
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
            var encodedJwt = _jwtProviderService.EncodeToken(_jwtProviderService.CreateJwtToken(createdUser,createdUserRoles));
            var response = new RegisterResponseDto(createdUser.Id, encodedJwt);
            return Ok(response);
        }

        [HttpPost("login")]
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
            var encodedJwt = _jwtProviderService.EncodeToken(_jwtProviderService.CreateJwtToken(loggedInUser, loggedInRoles));
            var response = new LoginResponseDto(loggedInUser.Id, encodedJwt);
            return Ok(Json(response));
        }
    }
}
