using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class CourseSearchResultDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double TotalHours { get; set; }
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public int StudentsCount { get; set; }
    public string? Level { get; set; }
    public PictureDto Picture { get; set; } = null!;
    public Discount Discount { get; set; } = null!;
    public string InstructorName { get; set; } = null!;
}
