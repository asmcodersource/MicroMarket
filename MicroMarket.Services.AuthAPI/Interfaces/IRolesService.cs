using CSharpFunctionalExtensions;
using MicroMarket.Services.AuthAPI.Models;

namespace MicroMarket.Services.AuthAPI.Interfaces
{
    public interface IRolesService
    {
        public Task<Result<IList<ApplicationUser>>> GetRoleUsers(string role);
        public Task<Result> AddRole(string role);
        public Task<Result> RemoveRole(string role);
        public Task<Result<IList<string>>> GetExistRoles();
        public Task<Result<IList<string>>> GetRoles(string userId);
        public Task<Result<IList<string>>> AssignRoles(string userId, IList<string> roles);
        public Task<Result<IList<string>>> UnassignRoles(string userId, IList<string> roles);
    }
}
