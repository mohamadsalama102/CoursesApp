using nagiashraf.CoursesApp.Services.Catalog.Core.Pagination;

namespace nagiashraf.CoursesApp.Services.Catalog.Data;

public static class AsyncIQueryableExtensions
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();

        var data = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedList<T>(data, pageIndex, pageSize, count);
    }
}
