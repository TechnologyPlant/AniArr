using System.Text.Json.Serialization;

namespace SyncSenpai.Server.Entities.External;

public class MediaListData
{
    [JsonPropertyName("MediaListCollection")]
    public MediaListCollection MediaListCollection { get; set; } = new();
}