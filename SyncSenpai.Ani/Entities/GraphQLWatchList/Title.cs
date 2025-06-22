using System.Text.Json.Serialization;

namespace SyncSenpai.Ani.Entities.GraphQLWatchList;

public class Title
{
    [JsonPropertyName("english")]
    public string English { get; set; } = "";
}
