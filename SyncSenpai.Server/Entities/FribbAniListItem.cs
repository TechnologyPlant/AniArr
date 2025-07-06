using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace SyncSenpai.Server.Entities;

public class FribbAniListItem
{
    [JsonPropertyName("type")]
    public string? ContentType { get; set; }

    [JsonPropertyName("anilist_id")]
    [BsonId]
    public int AniListId { get; set; }

    [JsonPropertyName("thetvdb_id")]
    public int? TvdbId { get; set; } = 0;
}
