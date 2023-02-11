namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class Quiz : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Section Section { get; set; } = null!;
    public int SectionId { get; set; }
    public List<Question> Questions { get; set; } = null!;
}