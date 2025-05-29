using System.Text.Json.Serialization;

namespace SyncSempai.Ani.Entities.External;

public class MediaListCollection
{
    [JsonPropertyName("lists")]
    public List<MediaList> Lists { get; set; } = [];
}
