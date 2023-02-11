using nagiashraf.CoursesApp.EventBus.EventBus.Events;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.IntegrationEvents;

public interface IEnrollingIntegrationEventService
{
    void PublishThroughEventBusAsync(IntegrationEvent evt);
}
