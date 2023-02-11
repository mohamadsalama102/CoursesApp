using nagiashraf.CoursesApp.Services.Catalog.Core.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class QuizDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(80)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [MinLength(1)]
    [HasUniqueOrders]
    public List<QuestionDto> Questions { get; set; } = null!;
}
