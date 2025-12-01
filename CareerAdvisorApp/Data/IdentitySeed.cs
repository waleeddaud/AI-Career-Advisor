using Microsoft.AspNetCore.Identity;      // for RoleManager, UserManager, IdentityUser, IdentityRole
using System.Security.Claims;              // for Claim
using Microsoft.Extensions.DependencyInjection; // for IServiceProvider
using System.Threading.Tasks; 
public static class IdentitySeed
{
    public static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
            await userManager.CreateAsync(adminUser, "Admin@123"); 
            await userManager.AddToRoleAsync(adminUser, "Admin");
            Claim _claim = new Claim("Permission", "FullAccess");
            await userManager.AddClaimAsync(adminUser, _claim);
        }
    }
}
