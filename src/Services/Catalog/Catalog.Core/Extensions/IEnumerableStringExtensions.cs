namespace nagiashraf.CoursesApp.Services.Catalog.Core.Extensions;

public static class IEnumerableStringExtensions
{
    public static string ToStringWithSeparator(this IEnumerable<string> list, string separator = "^^SEPARATOR^^")
        => string.Join(separator, list);
}
