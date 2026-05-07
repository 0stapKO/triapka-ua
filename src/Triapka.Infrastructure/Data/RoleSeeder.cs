using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

namespace Triapka.Infrastructure.Data;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { "Admin", "Customer" };

        foreach (var roleName in roleNames)
        {
            try
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    Log.Information("Role {RoleName} created successfully", roleName);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while creating role {RoleName}", roleName);
            }
        }
    }
}