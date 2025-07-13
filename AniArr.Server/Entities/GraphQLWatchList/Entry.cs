using System.Text.Json.Serialization;

namespace AniArr.Server.Entities.GraphQLWatchList;

public class Entry
{
    [JsonPropertyName("media")]
    public Media Media { get; set; } = new();
}
