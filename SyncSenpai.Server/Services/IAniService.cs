using SyncSenpai.Server.Entities.External.AnilistResponse.WatchList;

namespace SyncSenpai.Ani.Services
{
    public interface IAniService
    {
        Task<Root> GetUserWatchListAsync(string username);
    }
}