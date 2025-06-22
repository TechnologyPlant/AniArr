using SyncSenpai.Ani.Entities;
using SyncSenpai.Ani.Entities.GraphQLWatchList;

namespace SyncSenpai.Ani.Services
{
    public interface IAniService
    {
        Task<Root> GetUserWatchListAsync(string username);
        Task StoreAniUserAsync(AniUser aniUser);
    }
}