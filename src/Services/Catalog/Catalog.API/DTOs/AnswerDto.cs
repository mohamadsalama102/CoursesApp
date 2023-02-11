using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class AnswerDto
{
    public int Order { get; set; }

    [Required, MaxLength(600)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public bool IsCorrect { get; set; }

    [MaxLength(600)]
    public string? Explanation { get; set; }
}