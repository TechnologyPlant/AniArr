using System.Text.Json.Serialization;

namespace AniArr.Ani.Entities.GraphQLWatchList;

public class Title
{
    [JsonPropertyName("english")]
    public string English { get; set; } = "";
}
