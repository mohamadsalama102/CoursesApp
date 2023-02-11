using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;

public class QuestionDraftCreateDto : IValidatableObject
{
    [Required]
    public string Question { get; set; } = string.Empty;

    [Required]
    public int QuizId { get; set; }

    [Required]
    public int? RelatedLectureId { get; set; }

    [Required, MinLength(2)]
    public List<AnswerDraftDto> Answers { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Answers.Any(a => a.IsCorrect))
        {
            yield return new ValidationResult("Please choose at least one best answer.", new[] { nameof(Answers) });
        }
    }
}
