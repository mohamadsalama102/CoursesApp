using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class CourseDetailsDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Description { get; set; }
    public string? Topic { get; set; }
    public List<string>? WhatYouWillLearn { get; set; }
    public List<string>? Requirements { get; set; }
    public List<string>? WhoIsThisCourseFor { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastUpdated { get; set; }
    public double TotalHours { get; set; }
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public int StudentsCount { get; set; }
    public string? Level { get; set; }
    public string? Language { get; set; }
    public SubCategoryDto SubCategory { get; set; } = null!;
    public PictureDto? Picture { get; set; }
    public CoursePreviewVideoDto? PreviewVideo { get; set; }
    public Discount? Discount { get; set; }
    public InstructorDto Instructor { get; set; } = null!;
    public List<SectionDto> Sections { get; set; } = null!;
    public List<RatingCreateDto> Ratings { get; set; } = null!;
}
