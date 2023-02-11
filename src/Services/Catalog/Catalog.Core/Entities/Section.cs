namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class Section : BaseEntity
{
    public int? Order { get; set; }
    public string? Title { get; set; }
    public string? LearningObjective { get; set; }
    public Course Course { get; set; } = null!;
    public int CourseId { get; set; }
    public List<Lecture> Lectures { get; set; } = null!;
    public List<Quiz> Quizzes { get; set; } = null!;
}