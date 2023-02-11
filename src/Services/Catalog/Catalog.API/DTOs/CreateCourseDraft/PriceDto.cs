using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;

public class PriceDto
{
    [Required]
    public decimal Price { get; set; }

    [Required]
    public int CourseId { get; set; }
}
