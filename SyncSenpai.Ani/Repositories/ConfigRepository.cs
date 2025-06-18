using Marten;
using Microsoft.Extensions.Logging;
using SyncSenpai.Ani.Entities;

namespace SyncSenpai.Ani.Repositories
{
    public class ConfigRepository : IConfigRepository
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

        public async Task StoreFribbItem(List<FribbAniListItem> items)
        {
            try
            {
                await _documentStore.BulkInsertAsync(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, ex.Message);
                throw;
            }
        }
    }
}
