using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class LectureDto
{
    public int Id { get; set; }

    public int Order { get; set; }

    [Required, MaxLength(80)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public LectureVideoDto Video { get; set; } = null!;

    public List<DownloadableFileDto>? DownloadableFiles { get; set; }
}
