using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;

public class AnswerDraftDto
{
    [Required]
    public int Order { get; set; }
    [Required, MaxLength(600)]
    public string Answer { get; set; } = string.Empty;

    [Required]
    public bool IsCorrect { get; set; }

    [MaxLength(600)]
    public string? Explanation { get; set; }
}