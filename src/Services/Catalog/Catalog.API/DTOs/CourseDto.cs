using nagiashraf.CoursesApp.Services.Catalog.Core.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class CourseDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(60)]
    public string? Title { get; set; }

    [Required]
    [MaxLength(120)]
    public string? SubTitle { get; set; }

    [Required]
    [MinWordsCount(200)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(60)]
    public string? Topic { get; set; }

    [Required]
    [MinLength(4, ErrorMessage = "You must enter at least 4 learning objectives")]
    [MaxLengthForIEnumerableStringElement(160)]
    public List<string> WhatYouWillLearn { get; set; } = new();

    [Required]
    public List<string>? Requirements { get; set; }

    public List<string>? WhoIsThisCourseFor { get; set; }

    public decimal Price { get; set; }
    public double TotalHours { get; set; }

    [Required]
    public string? Level { get; set; }

    [Required]
    public string? Language { get; set; }

    public int SubCategoryId { get; set; }

    [Required]
    public string? InstructorId { get; set; }

    [Required]
    public PictureDto Picture { get; set; } = new();

    [Required]
    public CoursePreviewVideoDto PreviewVideo { get; set; } = null!;

    [Required]
    [MinLength(1)]
    [HasUniqueOrders]
    public List<SectionDto> Sections { get; set; } = new();
}