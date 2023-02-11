using Microsoft.AspNetCore.Identity;

namespace nagiashraf.CoursesApp.Services.Identity.API.Entities;

public class ApplicationUserRole : IdentityUserRole<string>
{
    public ApplicationUser User { get; set; } = null!;
    public ApplicationRole Role { get; set; } = null!;
}
