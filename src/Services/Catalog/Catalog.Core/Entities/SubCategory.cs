namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class SubCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Category Category { get; set; } = null!;
    public int CategoryId { get; set; }
    public List<Course> Courses { get; set; } = null!;
}