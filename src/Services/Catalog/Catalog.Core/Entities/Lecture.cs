namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class Lecture : BaseEntity
{
    public int? Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public LectureVideo Video { get; set; } = null!;
    public Section Section { get; set; } = null!;
    public int SectionId { get; set; }
    public List<DownloadableFile> DownloadableFiles { get; set; } = null!;
}