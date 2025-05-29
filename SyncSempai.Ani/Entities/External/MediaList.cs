using System.Text.Json.Serialization;

namespace SyncSempai.Ani.Entities.External;

public class MediaList
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("entries")]
    public List<Entry> Entries { get; set; } = [];
    [JsonPropertyName("status")]
    public string Status { get; set; } = "";
}
