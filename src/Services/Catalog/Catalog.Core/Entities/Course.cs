namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? SubTitle { get; set; }
    public string? Description { get; set; }
    public string? Topic { get; set; }
    public string? WhatYouWillLearn { get; set; }
    public string? Requirements { get; set; }
    public string? WhoIsThisCourseFor { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public double TotalHours { get; set; }
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public int StudentsCount { get; set; }
    public bool IsPublished { get; set; }
    public bool IsUnderAdminReview { get; set; }
    public bool IsRejected { get; set; }
    public Level? Level { get; set; }
    public Language? Language { get; set; }
    public SubCategory SubCategory { get; set; } = null!;
    public int SubCategoryId { get; set; }
    public Picture Picture { get; set; } = null!;
    public CoursePreviewVideo PreviewVideo { get; set; } = null!;
    public Discount Discount { get; set; } = null!;
    public ApplicationUser Instructor { get; set; } = null!;
    public string InstructorId { get; set; } = string.Empty;
    public List<Section> Sections { get; set; } = null!;
    public List<Rating> Ratings { get; set; } = null!;
    public List<ApplicationUser> Students { get; set; } = null!;
    public List<StudentCourse> StudentCourses { get; set; } = null!;
}
