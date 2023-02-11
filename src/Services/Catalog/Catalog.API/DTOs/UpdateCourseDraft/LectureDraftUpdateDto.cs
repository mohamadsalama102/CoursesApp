using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.UpdateCourseDraft;

public class LectureDraftUpdateDto
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Title { get; set; } = string.Empty;
}
