using System.Text.Json;
using nagiashraf.CoursesApp.Services.Identity.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace nagiashraf.CoursesApp.Services.Identity.API.Data;

public class AppIdentityDbContextSeed
{
    public async static Task SeedUsersAsync(UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
    {
        var contentRootPath = env.ContentRootPath;

        if (await userManager.Users.AnyAsync())
            return;

        List<ApplicationUser>? users = await GetDeserializedbjectsAsync<ApplicationUser>("ApplicationUsers.json", contentRootPath);
        if (users != null)
        {
            foreach (ApplicationUser user in users)
            {
                await userManager.CreateAsync(user, "P@$$w0rd");
                await userManager.AddToRoleAsync(user, "User");
            }
        }

        var adminUser = new ApplicationUser { UserName = "admin@domain.com", Email = "admin@domain.com", EmailConfirmed = true };
        await userManager.CreateAsync(adminUser, "P@$$w0rd");
        await userManager.AddToRolesAsync(adminUser, new[] { "Admin", "Moderator" });
    }

    public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, IWebHostEnvironment env)
    {
        var contentRootPath = env.ContentRootPath;

        if (await roleManager.Roles.AnyAsync())
            return;

        List<ApplicationRole>? roles = await GetDeserializedbjectsAsync<ApplicationRole>("ApplicationRoles.json", contentRootPath);
        if (roles != null)
        {
            foreach (ApplicationRole role in roles)
            {
                await roleManager.CreateAsync(role);
            }
        }
    }

    private async static Task<List<T>?> GetDeserializedbjectsAsync<T>(string fileName, string contentRootPath)
    {
        string jsonFilePath = Path.Combine(contentRootPath, "SeedData", fileName);
        string jsonObjects = await File.ReadAllTextAsync(jsonFilePath);

        return JsonSerializer.Deserialize<List<T>?>(jsonObjects);
    }
}