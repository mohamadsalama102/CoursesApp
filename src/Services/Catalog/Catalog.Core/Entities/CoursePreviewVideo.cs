namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class CoursePreviewVideo : BaseEntity
{
    public string? Url { get; set; }
    public string? PublicId { get; set; }
    public double? Duration { get; set; }
    public Course Course { get; set; } = null!;
    public int CourseId { get; set; }
}
