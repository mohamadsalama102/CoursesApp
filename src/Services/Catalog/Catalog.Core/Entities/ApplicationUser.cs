namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class ApplicationUser
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Headline { get; set; }
    public string? Biography { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PhotoPublicId { get; set; }
    public List<Course> CreatedCourses { get; set; } = null!;
    public List<Course> EnrolledInCourses { get; set; } = null!;
    public List<StudentCourse> StudentCourses { get; set; } = null!;
    public List<Rating> Ratings { get; set; } = null!;
}
