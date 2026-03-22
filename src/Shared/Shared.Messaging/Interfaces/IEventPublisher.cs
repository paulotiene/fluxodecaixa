namespace Shared.Messaging.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync(string payload, CancellationToken cancellationToken = default);
}
