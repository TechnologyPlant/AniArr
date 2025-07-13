using System.Text.Json.Serialization;

namespace AniArr.Ani.Entities.GraphQLWatchList;

public class MediaListCollection
{
    [JsonPropertyName("lists")]
    public List<MediaList> Lists { get; set; } = [];
}
