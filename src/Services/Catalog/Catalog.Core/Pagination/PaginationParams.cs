namespace nagiashraf.CoursesApp.Services.Catalog.Core.Pagination;

public class PaginationParams
{
    private const int MaxPageSize = 50;

    private int _pageIndex = 1;
    private int _pageSize = 20;

    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = value <= 0 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            _pageSize = value;
            if (_pageSize <= 0) _pageSize = 1;
            if (_pageSize > MaxPageSize) _pageSize = MaxPageSize;
        }
    }
}
