using MongoDB.Bson.Serialization.Attributes;

namespace AniArr.Server.Entities;

public class WatchlistItem
{
    [BsonId]
    public int TvdbId { get; set; }

    [BsonElement("aniListIds")]
    public HashSet<AniListItem> AniListItems { get; set; } = [];

    [BsonElement("title")]
    public string Title { get; set; } = "";
}
public class AniListItem:IEquatable<AniListItem>
{
    public int AniListId { get; set; }
    public string Title { get; set; } = "";

    bool IEquatable<AniListItem>.Equals(AniListItem? other)
    { 
        return this is not null && other is not null && AniListId == other.AniListId;
    }
    public override int GetHashCode()
    {
        return AniListId.GetHashCode();
    }
}
