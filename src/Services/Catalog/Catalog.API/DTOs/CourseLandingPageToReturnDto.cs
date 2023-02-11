using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class CourseLandingPageToReturnDto
{
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Description { get; set; }
    public string? Language { get; set; }
    public string? Level { get; set; }
    public string? Topic { get; set; }
    public string? PicturePublicId { get; set; }
    public string? PictureUrl { get; set; }
    public string? PreviewVideoPublicId { get; set; }
    public string? PreviewVideoUrl { get; set; }
    public int CourseId { get; set; }
}
