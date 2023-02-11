namespace nagiashraf.CoursesApp.Services.Identity.API.DTOs;

public class TokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
