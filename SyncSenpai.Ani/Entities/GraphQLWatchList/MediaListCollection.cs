using System.Text.Json.Serialization;

namespace SyncSenpai.Ani.Entities.GraphQLWatchList;

public class MediaListCollection
{
    [JsonPropertyName("lists")]
    public List<MediaList> Lists { get; set; } = [];
}
