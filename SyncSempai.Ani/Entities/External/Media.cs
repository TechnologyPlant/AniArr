using System.Text.Json.Serialization;

namespace SyncSempai.Ani.Entities.External;

public class Media
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("title")]
    public Title Title { get; set; } = new();
}
