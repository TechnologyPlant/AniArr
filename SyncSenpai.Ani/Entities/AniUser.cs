using Marten.Schema;

namespace SyncSenpai.Ani.Entities;

public class AniUser
{
    public int Id { get; set; } = 1;
    public List<WatchList> WatchList { get; set; } = [];
}
public class WatchList
{
    public required string Name { get; set; }
    public List<WatchListEntry> Entries { get; set; } = [];
}
public record WatchListEntry
{
    [Identity]
    public int AniListId { get; set; }
    public int TvDbId { get; set; }
}