using Marten;
using Microsoft.Extensions.Logging;
using SyncSenpai.Sonarr.Entities;

namespace SyncSenpai.Sonarr.Repositories
{
    public class SonarrConfigRepository
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<SonarrConfigRepository> _logger;

        public SonarrConfigRepository(IDocumentStore documentStore, ILogger<SonarrConfigRepository> logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        public async Task<SonarrConfig> GetConfigAsync()
        {
            await using var session = _documentStore.LightweightSession();
            var model = await session.Query<SonarrConfig>().SingleOrDefaultAsync();
            if (model is null)
            {
                _logger.LogDebug("Configuration requested but not found. Returning empty model");
            }
            return model ?? new();
        }
        public async Task StoreConfigAsync(SonarrConfig configModel)
        {
            await using var session = _documentStore.LightweightSession();
            session.Store(configModel);
            await session.SaveChangesAsync();
        }
    }
}
