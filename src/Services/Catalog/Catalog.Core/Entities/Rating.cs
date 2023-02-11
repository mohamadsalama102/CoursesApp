namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class Rating
{
    public int Id { get; set; }
    public int RatingValue { get; set; }
    public string? Review { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public ApplicationUser Student { get; set; } = null!;
    public string StudentId { get; set; } = string.Empty;
    public Course Course { get; set; } = null!;
    public int CourseId { get; set; }
}