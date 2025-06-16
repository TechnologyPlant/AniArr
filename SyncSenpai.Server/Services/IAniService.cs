using SyncSenpai.Server.Entities.External;

namespace SyncSenpai.Ani.Services
{
    public interface IAniService
    {
        Task<Root> GetUserWatchListAsync(string username);
    }
}