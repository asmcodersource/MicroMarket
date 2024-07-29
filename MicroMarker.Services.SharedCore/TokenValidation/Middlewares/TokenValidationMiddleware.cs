using MicroMarker.Services.SharedCore.TokenValidation.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMarker.Services.SharedCore.TokenValidation.Middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenValidationService _tokenValidationService;
        private readonly string _tokenPrefix = "Bearer ";

        public TokenValidationMiddleware(RequestDelegate next, TokenValidationService tokenValidationService) 
        {
            _next = next;
            _tokenValidationService = tokenValidationService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authorizationValue = context.Request.Headers["Authorization"].ToString();
            if (authorizationValue.StartsWith(_tokenPrefix))
            {
                var token = authorizationValue.Substring(_tokenPrefix.Length);
                var revokeResult = await _tokenValidationService.IsTokenRevoked(token);
                if (revokeResult.IsSuccess && !revokeResult.Value)
                    await _next(context);
                else if (revokeResult.IsSuccess && revokeResult.Value)
                    await context.Response.WriteAsJsonAsync("Token revoked");
                else
                    await context.Response.WriteAsJsonAsync("Token verification inernal server fault");
            } else
            {
                await _next(context);
            }
        }
    }
}
