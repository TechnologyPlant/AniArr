using System.Text.Json.Serialization;

namespace SyncSenpai.Server.Entities.External.AnilistResponse.WatchList;

public class Entry
{
    [JsonPropertyName("media")]
    public Media Media { get; set; } = new();
}
