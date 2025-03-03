using Microsoft.AspNetCore.Identity;
using ShopWebApplication.Models;

namespace ShopWebApplication.Initializer;

public static class RolesInitializer
{
    public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "admin", "user" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}