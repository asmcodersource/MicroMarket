namespace MicroMarket.Services.AuthAPI.Dtos
{
    public class LoginResponseDto
    {
        public string? UserId { get; }
        public string? JwtToken { get; }
        public object? Error { get; }
        public IList<string>? Roles { get; }

        public LoginResponseDto(string userId, string jwtToken, IList<string> roles)
        {
            UserId = userId;
            JwtToken = jwtToken;
            Roles = roles;
        }

        public LoginResponseDto(object error)
        {
            Error = error;
        }
    }
}
