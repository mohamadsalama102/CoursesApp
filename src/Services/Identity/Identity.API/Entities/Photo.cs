namespace nagiashraf.CoursesApp.Services.Identity.API.Entities;

public class Photo
{
    public int Id { get; set; }
    public string? Url { get; set; }
    public string? PublicId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
}
