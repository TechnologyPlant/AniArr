using System.Text.Json.Serialization;

namespace SyncSenpai.Ani.Entities.GraphQLWatchList;

public class Media
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("title")]
    public Title Title { get; set; } = new();
}
