using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;

public class QuizDraftCreateDto
{
    [Required, MaxLength(80)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int SectionId { get; set; }
}
