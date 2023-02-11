using nagiashraf.CoursesApp.Services.Enrolling.API.Data.Repositories;
using nagiashraf.CoursesApp.Services.Enrolling.API.Entities;

namespace nagiashraf.CoursesApp.Services.Enrolling.Tests.Data.Repositories;

public class EnrollmentRepositoryTests : IClassFixture<TestDatabaseFixture>
{
    public EnrollmentRepositoryTests(TestDatabaseFixture fixture)
        => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }

    [Fact]
    public async Task CanGetEnrollmentById()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var enrollmentRepository = new EnrollmentRepository(context);

        //Act
        Enrollment? enrollment = await enrollmentRepository.GetEnrollmentByIdAsync(1);

        //Assert
        Assert.NotNull(enrollment);
    }

    [Fact]
    public async Task CanGetUnsuccessfullEnrollmentForUser()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var enrollmentRepository = new EnrollmentRepository(context);

        //Act
        Enrollment? unsuccessfulEnrollment = await enrollmentRepository.GetUnsuccessfullEnrollmentForUserAsync("user ID 2");
        Enrollment? successfulEnrollment = await enrollmentRepository.GetUnsuccessfullEnrollmentForUserAsync("user ID 1");

        //Assert
        Assert.NotNull(unsuccessfulEnrollment);
        Assert.Null(successfulEnrollment);
    }

    [Fact]
    public async Task CanCreateEnrollment()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        await context.Database.BeginTransactionAsync();
        var enrollmentRepository = new EnrollmentRepository(context);

        var enrollmentToBeCreated = new Enrollment
        {
            StudentId = "user ID 2",
            CourseId = 1,
            CoursePrice = 19,
            PaymentSucceeded = true
        };

        //Act
        await enrollmentRepository.CreateEnrollmentAsync(enrollmentToBeCreated);

        context.ChangeTracker.Clear();

        //Assert
        var createdEnrollment = await enrollmentRepository.GetEnrollmentByIdAsync(enrollmentToBeCreated.Id);
        Assert.NotNull(createdEnrollment);
        Assert.Equal(createdEnrollment!.StudentId, enrollmentToBeCreated.StudentId);
    }

    [Fact]
    public async Task CanCheckIfEnrollmentExists()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var enrollmentRepository = new EnrollmentRepository(context);

        //Act
        var enrollmentExists = await enrollmentRepository.CheckIfSuccessfulEnrollmentExistsAsync("user ID 1", 1);
        var enrollmentDoesntExist = await enrollmentRepository.CheckIfSuccessfulEnrollmentExistsAsync("user ID 1", 2);

        //Assert
        Assert.True(enrollmentExists);
        Assert.False(enrollmentDoesntExist);
    }

    [Fact]
    public async Task CanSetTruePaymentSucceeded()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var enrollmentRepository = new EnrollmentRepository(context);
        var paymentNotSucceededEnrollment = await enrollmentRepository.GetEnrollmentByIdAsync(2);
        Assert.NotNull(paymentNotSucceededEnrollment);
        Assert.False(paymentNotSucceededEnrollment!.PaymentSucceeded);

        //Act
        await enrollmentRepository.SetTruePaymentSucceededAsync(paymentNotSucceededEnrollment);

        //Assert
        Assert.True(paymentNotSucceededEnrollment.PaymentSucceeded);
    }
}
