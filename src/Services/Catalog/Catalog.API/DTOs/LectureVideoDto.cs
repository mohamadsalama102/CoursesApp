using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class LectureVideoDto
{
    public int Id { get; set; }

    [Required]
    public string Url { get; set; } = string.Empty;

    [Required]
    public string PublicId { get; set; } = string.Empty;

    public List<SubtitleDto>? Subtitles { get; set; }
}