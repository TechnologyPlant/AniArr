using System.Text.Json.Serialization;

namespace SyncSempai.Ani.Entities.External;

public class Root
{
    [JsonPropertyName("data")]
    public MediaListData Data { get; set; } = new();
}
