using MockQueryable.Moq;
using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;
using nagiashraf.CoursesApp.Services.Catalog.Core.Pagination;
using nagiashraf.CoursesApp.Services.Catalog.Data;

namespace nagiashraf.CoursesApp.Services.Catalog.Tests.Data.Tests;

public class AsyncIQueryableExtensionsTests
{
    [Fact]
    public async void CanConvertToPagedList()
    {
        //Arragne
        var courses = new List<Course>() { new Course() };
        var iQueryableInput = courses.AsQueryable().BuildMock();
        var pageIndex = 1;
        var pageSize = 1;

        //Act
        var result = await iQueryableInput.ToPagedListAsync(pageIndex, pageSize);

        //Assert
        Assert.IsType<PagedList<Course>>(result);
    }
}
