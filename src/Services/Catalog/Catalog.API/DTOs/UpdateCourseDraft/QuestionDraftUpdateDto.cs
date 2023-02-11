using nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;
using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.UpdateCourseDraft;

public class QuestionDraftUpdateDto : IValidatableObject
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Question { get; set; } = string.Empty;

    public int? RelatedLectureId { get; set; }

    [Required, MinLength(2)]
    public List<AnswerDraftDto> Answers { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Answers.SingleOrDefault(a => a.IsCorrect) == null)
        {
            yield return new ValidationResult("Please choose one best answer.", new[] { nameof(Answers) });
        }
    }
}
