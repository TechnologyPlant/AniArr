using System.Text.Json.Serialization;

namespace AniArr.Ani.Entities.GraphQLWatchList;

public class Root
{
    [JsonPropertyName("data")]
    public MediaListData Data { get; set; } = new();
}
