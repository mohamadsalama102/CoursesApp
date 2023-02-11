using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Identity.API.DTOs;

public class LoginTwoFactorDto
{
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? TwoFactorAuthCode { get; set; }
}
