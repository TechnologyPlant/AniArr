using SyncSenpai.Ani.Entities;

namespace SyncSenpai.Ani.Repositories
{
    public interface IConfigRepository
    {
        Task<ConfigModel> GetConfigAsync();
        Task StoreConfigAsync(ConfigModel configModel);
    }
}