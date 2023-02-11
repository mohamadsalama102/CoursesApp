namespace nagiashraf.CoursesApp.EventBus.EventBus.Events;

public record IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }

    public Guid Id { get; private init; }
    public DateTime CreationDate { get; private init; }
}
