using System.Text.Json.Serialization;

namespace AniArr.Server.Entities.GraphQLWatchList;

public class Media
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("title")]
    public Title Title { get; set; } = new();
}
