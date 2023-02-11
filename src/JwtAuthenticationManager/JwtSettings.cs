namespace nagiashraf.CoursesApp.JwtAuthenticationManager;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public bool ValidateIssuer { get; set; }
    public string? ValidIssuer { get; set; }
    public bool ValidateAudience { get; set; }
    public string? ValidAudience { get; set; }
    public int TokenDurationInMinutes { get; set; }
    public int RefreshTokenDurationInDays { get; set; }
}
