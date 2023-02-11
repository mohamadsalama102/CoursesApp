using RabbitMQ.Client;

namespace nagiashraf.CoursesApp.EventBus.EventBusRabbitMQ;

public interface IRabbitMQPersistentConnection : IDisposable
{
    bool IsConnected { get; }
    bool TryConnect();
    IModel CreateModel();
}
