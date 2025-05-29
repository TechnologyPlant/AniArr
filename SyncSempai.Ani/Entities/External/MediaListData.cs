using System.Text.Json.Serialization;

namespace SyncSempai.Ani.Entities.External;

public class MediaListData
{
    [JsonPropertyName("MediaListCollection")]
    public MediaListCollection MediaListCollection { get; set; } = new();
}