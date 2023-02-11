namespace nagiashraf.CoursesApp.Services.Catalog.Core.Pagination;

public class PagedList<T> : List<T>
{
    public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
    {
        TotalCount = totalCount;
        TotalPages = TotalCount / pageSize;
        PageSize = pageSize;
        PageIndex = pageIndex;

        if (TotalCount % pageSize > 0)
            TotalPages++;

        AddRange(source);
    }

    public int PageIndex { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages { get; }
}
