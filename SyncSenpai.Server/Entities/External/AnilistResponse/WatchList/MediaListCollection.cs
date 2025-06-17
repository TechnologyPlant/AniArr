using System.Text.Json.Serialization;

namespace SyncSenpai.Server.Entities.External.AnilistResponse.WatchList;

public class MediaListCollection
{
    [JsonPropertyName("lists")]
    public List<MediaList> Lists { get; set; } = [];
}
