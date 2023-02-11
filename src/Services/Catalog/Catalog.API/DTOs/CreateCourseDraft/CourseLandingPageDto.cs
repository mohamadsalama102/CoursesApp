using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using nagiashraf.CoursesApp.Services.Catalog.Core.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs.CreateCourseDraft;

public class CourseLandingPageDto
{

    [MaxLength(60)]
    public string? Title { get; set; }

    [MaxLength(120)]
    public string? SubTitle { get; set; }

    [MinWordsCount(200)]
    public string? Description { get; set; }

    public Language? Language { get; set; }

    public Level? Level { get; set; }

    [MaxLength(60)]
    public string? Topic { get; set; }

    public IFormFile? Picture { get; set; }

    public IFormFile? PreviewVideo { get; set; }

    [Required]
    public int CourseId { get; set; }
}
