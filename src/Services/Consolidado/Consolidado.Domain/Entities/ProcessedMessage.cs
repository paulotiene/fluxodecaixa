namespace Consolidado.Domain.Entities;

public class ProcessedMessage
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public DateTime ProcessedOnUtc { get; private set; }

    private ProcessedMessage() { }

    public ProcessedMessage(Guid eventId)
    {
        Id = Guid.NewGuid();
        EventId = eventId;
        ProcessedOnUtc = DateTime.UtcNow;
    }
}
