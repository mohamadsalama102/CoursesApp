namespace nagiashraf.CoursesApp.Services.Catalog.Core.Pagination;

public class PaginationModel<T> where T : class
{
    public int PageIndex { get; private set; }

    public int PageSize { get; private set; }

    public int TotalCount { get; }

    public int TotalPages { get; }

    public IEnumerable<T> Data { get; private set; }

    public PaginationModel(int pageIndex, int pageSize, int totalCount, int totalPages, IEnumerable<T> data)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalPages;
        Data = data;
    }
}
