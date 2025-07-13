using System.Text.Json.Serialization;

namespace AniArr.Server.Entities.GraphQLWatchList;

public class MediaListCollection
{
    [JsonPropertyName("lists")]
    public List<MediaList> Lists { get; set; } = [];
}
