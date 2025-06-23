using SyncSenpai.Ani.Entities;
using SyncSenpai.Ani.Entities.GraphQLWatchList;

namespace SyncSenpai.Ani.Services
{
    public interface IAniService
    {
        Task<ConfigModel> GetConfigAsync();
        Task<List<WatchListEntry>> GetPendingEntriesAsync();
        Task<int> GetTvDbIdByAniListId(int aniListId);
        Task<Root> GetUserWatchListAsync(string username);
        Task StoreAniUserAsync(AniUser aniUser);
    }
}