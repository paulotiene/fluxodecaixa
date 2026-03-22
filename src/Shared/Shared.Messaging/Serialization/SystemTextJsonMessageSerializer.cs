using Shared.Messaging.Interfaces;
using System.Text.Json;

namespace Shared.Messaging.Serialization;

public sealed class SystemTextJsonMessageSerializer : IMessageSerializer
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    public string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

    public T Deserialize<T>(string value) => JsonSerializer.Deserialize<T>(value, Options)
        ?? throw new InvalidOperationException("Falha ao desserializar mensagem.");
}
