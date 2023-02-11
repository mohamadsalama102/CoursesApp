namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class Question : BaseEntity
{
    public int? Order { get; set; }
    public string Content { get; set; } = string.Empty;
    public Quiz Quiz { get; set; } = null!;
    public int QuizId { get; set; }
    public Lecture RelatedLecture { get; set; } = null!;
    public int? RelatedLectureId { get; set; }
    public List<Answer> Answers { get; set; } = null!;
}