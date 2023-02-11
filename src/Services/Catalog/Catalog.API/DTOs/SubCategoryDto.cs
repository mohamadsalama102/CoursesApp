using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

namespace nagiashraf.CoursesApp.Services.Catalog.API.DTOs;

public class SubCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CategoryDto Category { get; set; } = null!;
}
