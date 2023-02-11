using nagiashraf.CoursesApp.EventBus.EventBus.Abstractions;
using nagiashraf.CoursesApp.Services.Catalog.API.IntegrationEvents.Events;
using nagiashraf.CoursesApp.Services.Catalog.Data.Repositories;
using nagiashraf.CoursesApp.Services.Enrolling.API.Grpc;

namespace nagiashraf.CoursesApp.Services.Catalog.API.IntegrationEvents.EventHandling;

public class StudentEnrolledInCourseIntegrationEventHandler : IIntegrationEventHandler<StudentEnrolledInCourseIntegrationEvent>
{
	private readonly ILogger<StudentEnrolledInCourseIntegrationEventHandler> _logger;
	private readonly ICourseRepository _courseRepository;
    private readonly IIdentityGrpcClient _identityGrpcClient;

    public StudentEnrolledInCourseIntegrationEventHandler(ILogger<StudentEnrolledInCourseIntegrationEventHandler> logger,
		ICourseRepository courseRepository, IIdentityGrpcClient identityGrpcClient)
	{
		_logger = logger;
		_courseRepository = courseRepository;
        _identityGrpcClient = identityGrpcClient;
    }

    public async Task Handle(StudentEnrolledInCourseIntegrationEvent @event)
	{
        _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at Catalog Service - " +
            "({@IntegrationEvent})", @event.Id, @event);

        if (!await _courseRepository.UserExitsInCatalogServiceAsync(@event.StudentId))
        {
            var user = await _identityGrpcClient.GetUserDetailsAsync(@event.StudentId);

            if (user != null)
            await _courseRepository.CreateUserAsync(user);
        }
        await _courseRepository.EnrollStudentInCourseAsync(@event.StudentId, @event.CourseId);
	}
}
