using System.Text.Json.Serialization;

namespace AniArr.Ani.Entities.GraphQLWatchList;

public class MediaListData
{
    [JsonPropertyName("MediaListCollection")]
    public MediaListCollection MediaListCollection { get; set; } = new();
}