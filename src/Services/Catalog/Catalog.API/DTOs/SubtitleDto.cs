using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class SubtitleDto
{
    public int Id { get; set; }

    [Required]
    public Language Language { get; set; }

    [Required]
    public string FilePath { get; set; } = string.Empty;
}
