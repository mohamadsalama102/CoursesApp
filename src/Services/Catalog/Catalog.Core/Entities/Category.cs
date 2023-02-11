namespace nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public List<SubCategory> SubCategories { get; set; } = null!;
}
