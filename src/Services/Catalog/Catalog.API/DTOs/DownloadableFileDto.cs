using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class DownloadableFileDto
{
    public int Id { get; set; }

    [Required]
    public string? FileUrlPath { get; set; }
}