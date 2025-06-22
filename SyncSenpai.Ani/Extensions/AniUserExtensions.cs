using SyncSenpai.Ani.Entities;

namespace SyncSenpai.Ani.Extensions
{
    internal static class AniUserExtensions
    {
        public static List<WatchListEntry> GetNewEntries(this AniUser newUser, AniUser? oldUser)
        {
            if (oldUser is null) return [.. newUser.WatchList.SelectMany(x => x.Entries.Select(y => y))];
            else
            {
                List<WatchListEntry> oldEntries = [.. oldUser.WatchList.SelectMany(x => x.Entries.Select(y => y))];
                List<WatchListEntry> newEntries = [.. newUser.WatchList.SelectMany(x => x.Entries.Select(y => y))];
                return [.. newEntries.Except(oldEntries)];
            }
        }
    }
}
