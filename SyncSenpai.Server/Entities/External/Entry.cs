using System.Text.Json.Serialization;

namespace SyncSenpai.Server.Entities.External;

public class Entry
{
    [JsonPropertyName("media")]
    public Media Media { get; set; } = new();
}
