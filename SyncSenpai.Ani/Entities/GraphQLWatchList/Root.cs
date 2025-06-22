using System.Text.Json.Serialization;

namespace SyncSenpai.Ani.Entities.GraphQLWatchList;

public class Root
{
    [JsonPropertyName("data")]
    public MediaListData Data { get; set; } = new();
}
