using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using nagiashraf.CoursesApp.Services.Enrolling.API.Controllers;
using nagiashraf.CoursesApp.Services.Enrolling.API.Data.Repositories;
using nagiashraf.CoursesApp.Services.Enrolling.API.DTOs;
using nagiashraf.CoursesApp.Services.Enrolling.API.Entities;
using nagiashraf.CoursesApp.Services.Enrolling.API.Grpc;
using nagiashraf.CoursesApp.Services.Enrolling.API.IntegrationEvents;
using nagiashraf.CoursesApp.Services.Enrolling.API.Services.Payments;
using System.Net;
using System.Security.Claims;

namespace nagiashraf.CoursesApp.Services.Enrolling.Tests.Controllers;

public class EnrollmentControllerTests
{
    private readonly Mock<IEnrollmentRepository> _enrollmentRepositorMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly Mock<IEnrollingIntegrationEventService> _enrollingIntegrationEventServiceMock;
    private readonly Mock<ICatalogGrpcClient> _catalogGrpcClientMock;

    public EnrollmentControllerTests()
    {
        _enrollmentRepositorMock = new Mock<IEnrollmentRepository>();
        _paymentServiceMock = new Mock<IPaymentService>();
        _enrollingIntegrationEventServiceMock = new Mock<IEnrollingIntegrationEventService>();
        _catalogGrpcClientMock = new Mock<ICatalogGrpcClient>();
    }

    [Fact]
    public async Task CanGetEnrollmentById()
    {
        //Arrange
        var enrollmentId = 1;
        var enrollment = new Enrollment() { Id = enrollmentId };

        _enrollmentRepositorMock
            .Setup(s => s.GetEnrollmentByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);

        //Act
        var enrollmentController = new EnrollmentController(_enrollmentRepositorMock.Object, _paymentServiceMock.Object,
            _enrollingIntegrationEventServiceMock.Object, _catalogGrpcClientMock.Object);
        var actionResult = await enrollmentController.GetEnrollmentById(enrollmentId);

        //Assert
        _enrollmentRepositorMock.Verify(s => s.GetEnrollmentByIdAsync(enrollmentId));
        Assert.Equal(enrollmentId, actionResult?.Value?.Id);
    }

    [Fact]
    public async Task CanrCreateOrUpdateEnrollmentWithPaymentIntent()
    {
        //Arrange
        var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, "userId") };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var contextMock = new Mock<HttpContext>();
        contextMock
            .Setup(ctx => ctx.User)
            .Returns(claimsPrincipal);
        var controllerContext = new ControllerContext()
        {
            HttpContext = contextMock.Object
        };

        var courseDto = new CourseDto
        {
            CourseId = 1,
            Price = 59
        };

        _catalogGrpcClientMock
            .Setup(c => c.GetCoursePriceAsync(It.IsAny<int>()))
            .ReturnsAsync(courseDto.Price);

        _paymentServiceMock
            .Setup(s => s.CreateOrUpdateEnrollmentWithPaymentIntentAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<decimal>()))
            .ReturnsAsync(new Enrollment());

        _enrollmentRepositorMock
            .Setup(r => r.CreateEnrollmentAsync(It.IsAny<Enrollment>()))
            .ReturnsAsync(new Enrollment());

        //Act
        var enrollmentController = new EnrollmentController(_enrollmentRepositorMock.Object, _paymentServiceMock.Object,
            _enrollingIntegrationEventServiceMock.Object, _catalogGrpcClientMock.Object)
        {
            ControllerContext = controllerContext
        };

        var actionResult = await enrollmentController.CreateOrUpdateEnrollmentWithPaymentIntent(courseDto) as CreatedAtActionResult;

        //Assert
        _catalogGrpcClientMock.Verify(c => c.GetCoursePriceAsync(It.IsAny<int>()));
        _paymentServiceMock.Verify(s => s.CreateOrUpdateEnrollmentWithPaymentIntentAsync(It.IsAny<string>(), It.IsAny<int>(),
            It.IsAny<decimal>()));
        contextMock.Verify(ctx => ctx.User);
        Assert.Equal(actionResult?.StatusCode, (int)HttpStatusCode.Created);
    }
}
