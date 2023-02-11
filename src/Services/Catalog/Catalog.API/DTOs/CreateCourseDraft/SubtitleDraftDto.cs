using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;

public class SubtitleDraftDto
{
    [Required]
    public Language Language { get; set; }

    [Required]
    public IFormFile File { get; set; } = null!;

    [Required]
    public int VideoId { get; set; }
}
