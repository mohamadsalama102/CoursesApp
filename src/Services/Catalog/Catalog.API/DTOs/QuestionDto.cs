using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class QuestionDto
{
    public int Id { get; set; }

    public int Order { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required, MinLength(2)]
    public List<AnswerDto> Answers { get; set; } = null!;
}
