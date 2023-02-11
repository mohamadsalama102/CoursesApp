using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Identity.API.DTOs;

public class LoginDto
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
