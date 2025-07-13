using AniArr.Ani.Entities;
using AniArr.Ani.Entities.GraphQLWatchList;

namespace AniArr.Ani.Services
{
    public interface IAniService
    {
        Task<ConfigModel> GetConfigAsync();
        Task<List<WatchListEntry>> GetPendingEntriesAsync();
        Task<int> GetTvDbIdByAniListId(int aniListId);
        Task<Root> GetUserWatchListAsync(string username);
        Task StoreAniUserAsync(AniUser aniUser);
        Task StoreConfigAsync(ConfigModel configModel);
        Task StoreFribbItems(List<FribbAniListItem> items, ConfigModel configModel);
    }
}