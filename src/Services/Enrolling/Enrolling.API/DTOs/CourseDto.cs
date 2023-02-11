using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.DTOs;

public class CourseDto
{
    [Required]
    public int CourseId { get; set; }

    [Required]
    public decimal Price { get; set; }
}
