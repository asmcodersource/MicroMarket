using MicroMarket.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace MicroMarket.Services.Identity.DbContexts
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedData(IdentityDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(context, roleManager);
            await SeedUsers(context, userManager);
        }

        private static async Task SeedRoles(IdentityDbContext context, RoleManager<IdentityRole> roleManager)
        {
            var isAdminRolePresented = context.Roles.Where(r => r.NormalizedName == "Admin".ToUpperInvariant()).Any();
            if (isAdminRolePresented)
                return;
            string[] roles = ["Admin", "Manager", "Customer"];
            foreach (var role in roles)
            {
                IdentityRole identityRole = new IdentityRole(role);
                await roleManager.CreateAsync(identityRole);
            }
        }

        private static async Task SeedUsers(IdentityDbContext context, UserManager<ApplicationUser> userManager)
        {
            // Creating debug admin user
            var adminUser = new ApplicationUser();
            adminUser.UserName = "Admin";
            adminUser.Email = "admin@mail.com";
            await userManager.CreateAsync(adminUser, "Qwerty@123123");
            await userManager.AddToRoleAsync(adminUser, "Admin");
            await userManager.AddToRoleAsync(adminUser, "Customer");
            await userManager.AddToRoleAsync(adminUser, "Manager");
            // Creating debug admin user
            var customerUser = new ApplicationUser();
            customerUser.UserName = "Customer";
            customerUser.Email = "customer@mail.com";
            await userManager.CreateAsync(customerUser, "Qwerty@123123");
            await userManager.AddToRoleAsync(customerUser, "Customer");
        }
    }
}
