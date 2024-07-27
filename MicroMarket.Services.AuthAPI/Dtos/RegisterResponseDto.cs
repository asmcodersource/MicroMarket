namespace MicroMarket.Services.AuthAPI.Dtos
{
    public class RegisterResponseDto
    {
        public readonly string? UserId;
        public readonly string? JwtToken;
        public readonly object? Error;

        public RegisterResponseDto(string userId, string jwtToken)
        {
            UserId = userId;
            JwtToken = jwtToken;
        }

        public RegisterResponseDto(object error)
        {
            Error = error;
        }
    }
}
