using nagiashraf.CoursesApp.Services.Catalog.Core.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class SectionDto
{
    public int Id { get; set; }

    public int Order { get; set; }

    [Required, MaxLength(80)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? LearningObjective { get; set; }

    [Required, MinLength(2), HasUniqueOrders]
    public List<LectureDto> Lectures { get; set; } = new();

    [HasUniqueOrders]
    public List<QuizDto> Quizzes { get; set; } = new();
}
