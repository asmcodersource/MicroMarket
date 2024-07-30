using CSharpFunctionalExtensions;
using MicroMarket.Services.Identity.DbContexts;
using MicroMarket.Services.Identity.Interfaces;
using MicroMarket.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MicroMarket.Services.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _usersManager;
        private readonly AuthDbContext _authDbContext;

        public AuthService(AuthDbContext dbContext, UserManager<ApplicationUser> usersManager)
        {
            _authDbContext = dbContext;
            _usersManager = usersManager;
        }

        public async Task<Result<(ApplicationUser, IList<string>)>> Login(string email, string password)
        {
            try
            {
                var user = await _authDbContext.ApplicationUsers.SingleOrDefaultAsync(u => u.Email == email);
                if (user is null)
                    return Result.Failure<(ApplicationUser, IList<string>)>("User does not exist");

                bool isValid = await _usersManager.CheckPasswordAsync(user, password);
                if (!isValid)
                    return Result.Failure<(ApplicationUser, IList<string>)>("Wrong password provided");

                var roles = await _usersManager.GetRolesAsync(user);
                return Result.Success((user, roles));
            }
            catch (Exception ex)
            {
                return Result.Failure<(ApplicationUser, IList<string>)>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Result<(ApplicationUser, IList<string>)>> Register(string name, string surname, string middleName, string email, string password)
        {
            try
            {
                var isEmailAlreadyClaimed = await _authDbContext.ApplicationUsers.AnyAsync(u => u.Email == email);
                if (isEmailAlreadyClaimed)
                    return Result.Failure<(ApplicationUser, IList<string>)>("Email already taken by another user");

                ApplicationUser creatingUser = new()
                {
                    Name = name,
                    Surname = surname,
                    MiddleName = middleName,
                    UserName = name,
                    Email = email,
                };
                var creatingResult = await _usersManager.CreateAsync(creatingUser, password);
                if (!creatingResult.Succeeded)
                    return Result.Failure<(ApplicationUser, IList<string>)>(string.Join(". ", creatingResult.Errors));

                var createdUser = await _authDbContext.ApplicationUsers.SingleAsync(u => u.Email == email)!;
                var roles = await _usersManager.GetRolesAsync(createdUser);
                return Result.Success((createdUser, roles));
            }
            catch (Exception ex)
            {
                return Result.Failure<(ApplicationUser, IList<string>)>($"An error occurred: {ex.Message}");
            }
        }
    }
}
