namespace Shared.Messaging.Interfaces;

public interface IMessageSerializer
{
    string Serialize<T>(T value);
    T Deserialize<T>(string value);
}
