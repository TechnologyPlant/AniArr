using System.Text.Json.Serialization;

namespace AniArr.Server.Entities.GraphQLWatchList;

public class Title
{
    [JsonPropertyName("english")]
    public string English { get; set; } = "";
}
