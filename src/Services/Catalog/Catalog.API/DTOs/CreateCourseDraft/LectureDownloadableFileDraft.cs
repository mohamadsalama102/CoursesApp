using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;

public class LectureDownloadableFileDraft
{
    [Required]
    public IFormFile File { get; set; } = null!;

    [Required]
    public int LectureId { get; set; }
}
