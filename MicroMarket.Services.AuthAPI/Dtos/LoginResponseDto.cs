namespace MicroMarket.Services.AuthAPI.Dtos
{
    public class LoginResponseDto
    {
        public readonly string? UserId;
        public readonly string? JwtToken;
        public readonly object? Error;

        public LoginResponseDto(string userId, string jwtToken)
        {
            UserId = userId;
            JwtToken = jwtToken;
        }

        public LoginResponseDto(object error)
        {
            Error = error;
        }
    }
}
