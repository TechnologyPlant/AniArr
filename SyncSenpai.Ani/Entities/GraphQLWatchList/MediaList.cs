using System.Text.Json.Serialization;

namespace SyncSenpai.Ani.Entities.GraphQLWatchList;

public class MediaList
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("entries")]
    public List<Entry> Entries { get; set; } = [];
    [JsonPropertyName("status")]
    public string Status { get; set; } = "";
}
