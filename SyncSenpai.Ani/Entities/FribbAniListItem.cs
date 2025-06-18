using Marten.Schema;
using Newtonsoft.Json;

namespace SyncSenpai.Ani.Entities;

public class FribbAniListItem
{
    [JsonProperty("type")]
    public string? ContentType { get; set; }

    [JsonProperty("anilist_id")]
    [Identity]
    public int AniListId { get; set; }

    [JsonProperty("thetvdb_id")]
    public int TvdbId { get; set; }
}
