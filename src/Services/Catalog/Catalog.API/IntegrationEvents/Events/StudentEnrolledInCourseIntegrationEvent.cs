using nagiashraf.CoursesApp.EventBus.EventBus.Events;

namespace nagiashraf.CoursesApp.Services.Catalog.API.IntegrationEvents.Events;

public record StudentEnrolledInCourseIntegrationEvent : IntegrationEvent
{
    public string StudentId { get; private init; }
    public int CourseId { get; private init; }

    public StudentEnrolledInCourseIntegrationEvent(string studentId, int courseId)
    {
        StudentId = studentId;
        CourseId = courseId;
    }
}
