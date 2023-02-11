using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class RatingCreateDto
{
    [Required, Range(1, 5)]
    public int RatingValue { get; set; }

    public string? Review { get; set; }

    public int CourseId { get; set; }
}
