using System.Text.Json.Serialization;

namespace SyncSenpai.Ani.Entities.GraphQLWatchList;

public class MediaListData
{
    [JsonPropertyName("MediaListCollection")]
    public MediaListCollection MediaListCollection { get; set; } = new();
}