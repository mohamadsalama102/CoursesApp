namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class Answer
{
    public int? Order { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string? Explanation { get; set; }
    public Question Question { get; set; } = null!;
    public int QuestionId { get; set; }
}