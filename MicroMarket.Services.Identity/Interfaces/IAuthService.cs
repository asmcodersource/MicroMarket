using CSharpFunctionalExtensions;
using MicroMarket.Services.Identity.Models;

namespace MicroMarket.Services.Identity.Interfaces
{
    public interface IAuthService
    {
        public Task<Result<(ApplicationUser, IList<string>)>> Login(string email, string password);
        public Task<Result<(ApplicationUser, IList<string>)>> Register(string name, string surname, string middleName, string email, string password);
    }
}
