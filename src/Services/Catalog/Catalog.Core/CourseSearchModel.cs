using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using nagiashraf.CoursesApp.Services.Catalog.Core.FilteringAndSorting;
using nagiashraf.CoursesApp.Services.Catalog.Core.Pagination;

namespace nagiashraf.CoursesApp.Services.Catalog.Core;

public class CourseSearchModel : PaginationParams
{
    public string? q { get; set; }
    public int? CategoryId { get; set; }
    public string? Topic { get; set; }
    public double? MinimumRating { get; set; }
    public PriceEnum? Price { get; set; }
    public Level[]? Level { get; set; }
    public Language[]? Language { get; set; }
    public DurationEnum[]? Duration { get; set; }
    public Language[]? SubtitleLanguage { get; set; }
    public CourseFeaturesEnum[]? Features { get; set; }
    public CourseSortingEnum? OrderBy { get; set; } = CourseSortingEnum.HighestRated;
}
