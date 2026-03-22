namespace Shared.Messaging.Events;

public abstract record IntegrationEvent(Guid EventId, DateTime OccurredAtUtc);
