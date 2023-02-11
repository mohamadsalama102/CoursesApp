using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.UpdateCourseDraft;

public class SectionDraftUpdateDto
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? LearningObjective { get; set; }
}
