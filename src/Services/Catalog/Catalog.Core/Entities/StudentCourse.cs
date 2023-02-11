namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class StudentCourse
{
    public Course Course { get; set; } = null!;
    public int CourseId { get; set; }
    public ApplicationUser Student { get; set; } = null!;
    public string StudentId { get; set; } =string.Empty;
}
