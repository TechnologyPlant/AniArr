using System.Text.Json.Serialization;

namespace SyncSempai.Ani.Entities.External;

public class Entry
{
    [JsonPropertyName("media")]
    public Media Media { get; set; } = new();
}
