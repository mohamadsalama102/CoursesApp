using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class RatingUpdateDto
{
    public int Id { get; set; }

    [Required, Range(1, 5)]
    public int RatingValue { get; set; }

    public string? Review { get; set; }
}
