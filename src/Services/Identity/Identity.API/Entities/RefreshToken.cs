namespace nagiashraf.CoursesApp.Services.Identity.API.Entities;

public class RefreshToken
{
    public string? Token { get; set; }
    public DateTime? ExpirationDate { get; set; }
}
