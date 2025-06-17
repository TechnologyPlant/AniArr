using System.Text.Json.Serialization;

namespace SyncSenpai.Server.Entities.External.AnilistResponse.WatchList;

public class Title
{
    [JsonPropertyName("english")]
    public string English { get; set; } = "";
}
