using CSharpFunctionalExtensions;
using MicroMarket.Services.Identity.DbContexts;
using MicroMarket.Services.Identity.Interfaces;
using MicroMarket.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MicroMarket.Services.Identity.Services
{
    public class RolesService : IRolesService
    {
        private readonly IdentityDbContext _authDbContext;
        private readonly RoleManager<IdentityRole> _rolesManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> rolesManager, IdentityDbContext authDbContext)
        {
            _authDbContext = authDbContext;
            _rolesManager = rolesManager;
            _userManager = userManager;
        }

        public async Task<Result<IList<string>>> AssignRoles(string userId, IList<string> roles)
        {
            try
            {
                var user = await _authDbContext.Users.SingleAsync(u => u.Id == userId);
                var normalizedRoles = roles.Select(r => r.ToUpperInvariant()).ToList();
                foreach (var role in normalizedRoles)
                    if (!await _rolesManager.RoleExistsAsync(role))
                        await _rolesManager.CreateAsync(new IdentityRole(role));
                await _userManager.AddToRolesAsync(user, normalizedRoles);
                return Result.Success(await _userManager.GetRolesAsync(user));
            }
            catch (Exception ex)
            {
                return Result.Failure<IList<string>>(ex.Message);
            }
        }

        public async Task<Result<IList<string>>> UnassignRoles(string userId, IList<string> roles)
        {
            try
            {
                var user = await _authDbContext.Users.SingleAsync(u => u.Id == userId);
                await _userManager.RemoveFromRolesAsync(user, roles.Select(r => r.ToUpperInvariant()).ToList());
                return Result.Success(await _userManager.GetRolesAsync(user));
            }
            catch (Exception ex)
            {
                return Result.Failure<IList<string>>(ex.Message);
            }
        }

        public async Task<Result<IList<string>>> GetRoles(string userId)
        {
            try
            {
                var user = await _authDbContext.Users.SingleAsync(u => u.Id == userId);
                return Result.Success(await _userManager.GetRolesAsync(user));
            }
            catch (Exception ex)
            {
                return Result.Failure<IList<string>>(ex.Message);
            }
        }

        public async Task<Result<IList<ApplicationUser>>> GetRoleUsers(string role)
        {
            try
            {
                return Result.Success(await _userManager.GetUsersInRoleAsync(role.ToUpperInvariant()));
            }
            catch (Exception ex)
            {
                return Result.Failure<IList<ApplicationUser>>(ex.Message);
            }
        }

        public async Task<Result> AddRole(string role)
        {
            try
            {
                var isRoleAlreadyExist = await _rolesManager.Roles.Where(r => r.NormalizedName == role.ToUpperInvariant()).AnyAsync();
                if (isRoleAlreadyExist)
                    return Result.Failure($"Role {role} is already exists");
                await _rolesManager.CreateAsync(new IdentityRole(role));
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result> RemoveRole(string role)
        {
            try
            {
                var identityRole = await _rolesManager.Roles.Where(r => r.NormalizedName == role.ToUpperInvariant()).SingleOrDefaultAsync();
                if (identityRole is null)
                    return Result.Failure($"Role {role} is not exists");
                await _rolesManager.DeleteAsync(identityRole);
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }
        }

        public async Task<Result<IList<string?>>> GetExistRoles()
        {
            try
            {
                var roles = await _rolesManager.Roles.Select(r => r.Name).AsNoTracking().ToListAsync() ?? new List<string?>();
                return Result.Success<IList<string?>>(roles);
            }
            catch (Exception ex)
            {
                return Result.Failure<IList<string?>>(ex.Message);
            }
        }
    }
}
