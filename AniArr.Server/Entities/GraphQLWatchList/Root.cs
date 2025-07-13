using System.Text.Json.Serialization;

namespace AniArr.Server.Entities.GraphQLWatchList;

public class Root
{
    [JsonPropertyName("data")]
    public MediaListData Data { get; set; } = new();
}
