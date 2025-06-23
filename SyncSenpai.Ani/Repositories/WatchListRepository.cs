using Marten;
using Microsoft.Extensions.Logging;
using SyncSenpai.Ani.Entities;
using SyncSenpai.Ani.Extensions;

namespace SyncSenpai.Ani.Repositories;

public class WatchListRepository
{

    private readonly IDocumentStore _documentStore;
    private readonly ILogger<WatchListRepository> _logger;

    public WatchListRepository(IDocumentStore documentStore, ILogger<WatchListRepository> logger)
    {
        _documentStore = documentStore;
        _logger = logger;
    }

    public async Task<AniUser?> GetAniUserAsync()
    {
        await using var session = _documentStore.LightweightSession();
        var user = await session.Query<AniUser>().SingleOrDefaultAsync();
        if (user is null)
        {
            _logger.LogDebug("AniUser requested but not found. Returning null");
        }
        return user;
    }

    public async Task StoreAniUserAsync(AniUser aniUser)
    {
        var existingUser = await GetAniUserAsync();

        await using var session = _documentStore.LightweightSession();
        session.Store(aniUser);

        var newEntries = aniUser.GetNewEntries(existingUser);
        await _documentStore.BulkInsertAsync(newEntries);

        await session.SaveChangesAsync();

    }
    public async Task RemoveWatchListEntry(WatchListEntry watchListEntry)
    {
        await using var session = _documentStore.LightweightSession();
        session.Delete(watchListEntry);
        await session.SaveChangesAsync();
    }

    public async Task<List<WatchListEntry>> GetPendingEntriesAsync()
    {
        return [.. (await _documentStore.QuerySerializableSessionAsync()).Query<WatchListEntry>()];
    }
}
