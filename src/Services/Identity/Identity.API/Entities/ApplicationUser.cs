using Microsoft.AspNetCore.Identity;

namespace nagiashraf.CoursesApp.Services.Identity.API.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Headline { get; set; }
    public string? Biography { get; set; }
    public string? Website { get; set; }
    public string? TwitterProfile { get; set; }
    public string? FacebookProfile { get; set; }
    public string? LinkedInProfile { get; set; }
    public string? YoutubeProfile { get; set; }
    public RefreshToken RefreshToken { get; set; } = null!;
    public Photo Photo { get; set; } = null!;
    public List<ApplicationUserRole> UserRoles { get; set; } = null!;
}
