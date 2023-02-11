using Microsoft.Extensions.Logging;
using Moq;
using nagiashraf.CoursesApp.Services.Catalog.API.IntegrationEvents.EventHandling;
using nagiashraf.CoursesApp.Services.Catalog.API.IntegrationEvents.Events;
using nagiashraf.CoursesApp.Services.Catalog.Data.Repositories;
using nagiashraf.CoursesApp.Services.Enrolling.API.Grpc;

namespace nagiashraf.CoursesApp.Services.Catalog.Tests.Api.Tests.EventHandling;

public class StudentEnrolledInCourseIntegrationEventHandlerTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly Mock<ILogger<StudentEnrolledInCourseIntegrationEventHandler>> _loggerMock;
    private readonly Mock<IIdentityGrpcClient> _identityGrpcClientMock;
    public StudentEnrolledInCourseIntegrationEventHandlerTests()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _loggerMock = new Mock<ILogger<StudentEnrolledInCourseIntegrationEventHandler>>();
        _identityGrpcClientMock = new Mock<IIdentityGrpcClient>();
    }

    [Fact]
    public async Task CanHandle()
    {
        var studentId = "student ID";
        var courseId = 123;

        var studentEnrolledInCourseIntegrationEvent = new StudentEnrolledInCourseIntegrationEvent(studentId, courseId);

        //Act
        var handler = new StudentEnrolledInCourseIntegrationEventHandler(_loggerMock.Object, _courseRepositoryMock.Object,
            _identityGrpcClientMock.Object);
        await handler.Handle(studentEnrolledInCourseIntegrationEvent);

        //Assert
        _courseRepositoryMock.Verify(r => r.EnrollStudentInCourseAsync(It.IsAny<string>(), It.IsAny<int>()));
    }
}
