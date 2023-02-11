namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class LectureVideo
{
    public int Id { get; set; }
    public string? Url { get; set; }
    public string? PublicId { get; set; }
    public double Duration { get; set; }
    public Lecture Lecture { get; set; } = null!;
    public int LectureId { get; set; }
    public List<Subtitle> Subtitles { get; set; } = null!;
}
