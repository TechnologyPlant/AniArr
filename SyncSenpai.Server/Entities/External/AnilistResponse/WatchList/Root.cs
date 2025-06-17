using System.Text.Json.Serialization;

namespace SyncSenpai.Server.Entities.External.AnilistResponse.WatchList;

public class Root
{
    [JsonPropertyName("data")]
    public MediaListData Data { get; set; } = new();
}
