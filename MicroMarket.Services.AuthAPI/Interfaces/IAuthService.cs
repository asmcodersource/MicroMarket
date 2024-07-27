using MicroMarket.Services.AuthAPI.Models;
using CSharpFunctionalExtensions;

namespace MicroMarket.Services.AuthAPI.Interfaces
{
    public interface IAuthService
    {
        public Task<Result<(ApplicationUser, IList<string>)>> Login(string email, string password);
        public Task<Result<(ApplicationUser, IList<string>)>> Register(string name, string surname, string middleName, string email, string password);
        public Task<Result> AssignRole(ApplicationUserRoles user);
        public Task<Result> UnassignRole(ApplicationUserRoles user);
    }
}
