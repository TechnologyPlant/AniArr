using Marten;
using Microsoft.Extensions.Logging;
using SyncSenpai.Ani.Entities;

namespace SyncSenpai.Ani.Repositories
{
    public class ConfigRepository
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<ConfigRepository> _logger;

        public ConfigRepository(IDocumentStore documentStore, ILogger<ConfigRepository> logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        public async Task<ConfigModel> GetConfigAsync()
        {
            await using var session = _documentStore.LightweightSession();
            var model = await session.Query<ConfigModel>().SingleOrDefaultAsync();
            if (model is null)
            {
                _logger.LogDebug("Configuration requested but not found. Returning empty model");
            }
            return model ?? new();
        }
        public async Task StoreConfigAsync(ConfigModel configModel)
        {
            await using var session = _documentStore.LightweightSession();
            session.Store(configModel);
            await session.SaveChangesAsync();
        }

        public async Task StoreFribbItems(List<FribbAniListItem> items, ConfigModel configModel)
        {
            try
            {
                await _documentStore.BulkInsertAsync(items);
                configModel.FribbListLastUpdated = DateTime.Now;
                await StoreConfigAsync(configModel);
                _logger.LogInformation($"timezone is {TimeZoneInfo.Local}");
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                throw;
            }
        }
        public async Task<int> GetTvDbIdByAniListId(int anilistId)
        {
            await using var session = _documentStore.LightweightSession();
            var model = await session.Query<FribbAniListItem>().SingleOrDefaultAsync(x => x.AniListId == anilistId);
            return model?.TvdbId ?? 0;
        }

        public async Task SetAniListUserNameAsync(string username)
        {
            var config = await GetConfigAsync();
            config.UserName = username;
            await StoreConfigAsync(config);
        }
    }
}
