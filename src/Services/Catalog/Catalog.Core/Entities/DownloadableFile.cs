namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class DownloadableFile : BaseEntity
{
    public string FileUrlPath { get; set; } = string.Empty;
    public Lecture Lecture { get; set; } = null!;
    public int LectureId { get; set; }
}
