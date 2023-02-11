using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;

public class LectureVideoDraftDto
{
    [Required]
    public IFormFile VideoFile { get; set; } = null!;

    [Required]
    public int LectureId { get; set; }
}
