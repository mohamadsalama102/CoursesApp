using nagiashraf.CoursesApp.EventBus.EventBus.Abstractions;
using nagiashraf.CoursesApp.EventBus.EventBus.Events;

namespace nagiashraf.CoursesApp.Services.Enrolling.API.IntegrationEvents;

public class EnrollingIntegrationEventService : IEnrollingIntegrationEventService
{
    private readonly ILogger<EnrollingIntegrationEventService> _logger;
    private readonly IEventBus _eventBus;

    public EnrollingIntegrationEventService(ILogger<EnrollingIntegrationEventService> logger, IEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
    }

    public void PublishThroughEventBusAsync(IntegrationEvent evt)
    {
        try
        {
            _logger.LogInformation("----- Publishing integration event: {IntegrationEventId_published} from Enrolling" +
                " Service - ({@IntegrationEvent})", evt.Id, evt);

            _eventBus.Publish(evt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from Enrolling" +
                " Service - ({@IntegrationEvent})", evt.Id, evt);
        }
    }
}
