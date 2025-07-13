using System.Text.Json.Serialization;

namespace AniArr.Server.Entities.GraphQLWatchList;

public class MediaListData
{
    [JsonPropertyName("MediaListCollection")]
    public MediaListCollection MediaListCollection { get; set; } = new();
}