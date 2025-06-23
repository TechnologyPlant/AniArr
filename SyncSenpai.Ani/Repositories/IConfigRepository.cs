using SyncSenpai.Ani.Entities;

namespace SyncSenpai.Ani.Repositories
{
    public interface IConfigRepository
    {
        Task<ConfigModel> GetConfigAsync();
        Task<int> GetTvDbIdByAniListId(int anilistId);
        Task StoreConfigAsync(ConfigModel configModel);
        Task StoreFribbItems(List<FribbAniListItem> items, ConfigModel configModel);
    }
}