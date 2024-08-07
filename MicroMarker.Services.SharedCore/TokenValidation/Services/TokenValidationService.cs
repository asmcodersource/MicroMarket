using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Distributed;

namespace MicroMarket.Services.SharedCore.TokenValidation.Services
{
    public class TokenValidationService
    {
        private readonly IDistributedCache _distributeCache;

        public TokenValidationService(IDistributedCache distributedCache)
        {
            _distributeCache = distributedCache;
        }

        public async Task<Result<bool>> IsTokenRevoked(string token)
        {
            var tokenRevoked = (await _distributeCache.GetAsync(token)) is not null;
            return Result.Success<bool>(tokenRevoked);
        }
    }
}
