using System.Text.Json.Serialization;

namespace SyncSenpai.Server.Entities.External.AnilistResponse.WatchList;

public class Media
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("title")]
    public Title Title { get; set; } = new();
}
