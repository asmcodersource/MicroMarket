namespace MicroMarket.Services.AuthAPI.Dtos
{
    public class RegisterResponseDto
    {
        public string? UserId { get; }
        public string? JwtToken { get; }
        public object? Error { get; }
        public IList<string>? Roles { get; }

        public RegisterResponseDto(string userId, string jwtToken, IList<string> roles)
        {
            UserId = userId;
            JwtToken = jwtToken;
            Roles = roles;
        }

        public RegisterResponseDto(object error)
        {
            Error = error;
        }
    }
}
