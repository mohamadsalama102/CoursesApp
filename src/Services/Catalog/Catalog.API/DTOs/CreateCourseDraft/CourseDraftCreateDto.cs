using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;

public class CourseDraftCreateDto
{
    [Required]
    public string CourseTitle { get; set; } = string.Empty;

    [Required]
    public int SubcategoryId { get; set; }
}
