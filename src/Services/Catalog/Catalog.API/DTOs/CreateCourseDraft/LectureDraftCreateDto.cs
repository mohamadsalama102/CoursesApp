using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;

public class LectureDraftCreateDto
{
    [Required, MaxLength(80)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public int SectionId { get; set; }
}
