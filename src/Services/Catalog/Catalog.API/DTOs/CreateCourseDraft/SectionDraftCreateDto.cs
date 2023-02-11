using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;

public class SectionDraftCreateDto
{
    [Required, MaxLength(80)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? LearningObjective { get; set; }

    [Required]
    public int CourseId { get; set; }
}