namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class Subtitle : BaseEntity
{
    public Language Language { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public LectureVideo Video { get; set; } = null!;
    public int VideoId { get; set; }
}