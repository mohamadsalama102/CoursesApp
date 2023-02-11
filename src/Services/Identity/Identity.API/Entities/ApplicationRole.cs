using Microsoft.AspNetCore.Identity;

namespace nagiashraf.CoursesApp.Services.Identity.API.Entities;

public class ApplicationRole : IdentityRole
{
    public List<ApplicationUserRole> UserRoles { get; set; } = null!;
}
