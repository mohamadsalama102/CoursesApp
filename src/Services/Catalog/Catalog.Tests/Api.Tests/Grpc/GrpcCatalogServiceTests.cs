using Grpc.Core;
using Moq;
using nagiashraf.CoursesApp.Services.Catalog.API.Grpc;
using nagiashraf.CoursesApp.Services.Catalog.API.Protos;
using nagiashraf.CoursesApp.Services.Catalog.Data.Repositories;

namespace nagiashraf.CoursesApp.Services.Catalog.Tests.Api.Tests.Grpc;

public class GrpcCatalogServiceTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    public GrpcCatalogServiceTests()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
    }

    [Fact]
    public async Task CanGetCoursePrice()
    {
        //Arrange
        var courseId = 1;
        var coursePrice = 10M;

        var getCoursePriceRequest = new GetCoursePriceRequest { CourseId = courseId };
        var serverCallContextMock = new Mock<ServerCallContext>();

        _courseRepositoryMock
            .Setup(r => r.GetCoursePriceAsync(It.IsAny<int>()))
            .ReturnsAsync(coursePrice);

        //Act
        var grpcCatalogService = new GrpcCatalogService(_courseRepositoryMock.Object);
        var response = await grpcCatalogService.GetCoursePrice(getCoursePriceRequest, serverCallContextMock.Object);

        //Assert
        _courseRepositoryMock.Verify(r => r.GetCoursePriceAsync(It.IsAny<int>()));
        Assert.Equal(coursePrice, response.Price, 9);
    }
}
