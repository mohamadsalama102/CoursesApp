using System.Security.Claims;

namespace nagiashraf.CoursesApp.Services.Catalog.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetIdFromPrincipal(this ClaimsPrincipal user)
    {
        return user?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}