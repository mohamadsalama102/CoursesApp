namespace nagiashraf.CoursesApp.Services.Identity.API.DTOs;

public class UserDto
{
    public string? UserId { get; set; }
    public string? AccessToken { get; set; }
    public DateTime? AccessTokenExpirationTime { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpirationTime { get; set; }
}
