using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class InstructorDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Headline { get; set; }
    public string? Biography { get; set; }
    public double InstructorRating { get; set; }
    public int RatingsCount { get; set; }
    public int StudentsCount { get; set; }
    public int CoursesCount { get; set; }
    public PictureDto? Photo { get; set; }
    public List<CourseSearchResultDto>? CreatedCourses { get; set; }
}
