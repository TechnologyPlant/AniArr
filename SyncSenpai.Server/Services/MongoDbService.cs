using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NetTopologySuite.Index.HPRtree;
using SyncSenpai.Server.Entities;
using System.Reflection;
using System.Text.Json;

namespace SyncSenpai.Server.Services;
public class MongoDbService
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<MongoDbService> _logger;
    private readonly AniService _aniService;

    public MongoDbService(IOptions<MongoSettings> mongoOptions, ILogger<MongoDbService> logger, AniService aniService)
    {
        var settings = mongoOptions.Value;

        var credentials = string.IsNullOrWhiteSpace(settings.Username)
            ? ""
            : $"{settings.Username}:{settings.Password}@";

        var connectionString = $"mongodb://{credentials}{settings.Host}:{settings.Port}";
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(settings.Database);
        _logger = logger;
        _aniService = aniService;
    }

    public async Task<AnilistConfigModel> GetConfigAsync()
    {
        var collection = _database.GetCollection<AnilistConfigModel>("config");
        var config = await collection.Find(x => x.Id == "serviceConfig").FirstOrDefaultAsync();
        return config ?? new();
    }

    public async Task SetAniListUserNameAsync(string username)
    {
        var filter = Builders<AnilistConfigModel>.Filter.Eq(x => x.Id, "serviceConfig");

        var collection = _database.GetCollection<AnilistConfigModel>("config");
        var update = Builders<AnilistConfigModel>.Update
            .Set(x => x.UserName, username);
        var updateOptions = new UpdateOptions { IsUpsert = true };

        var result = await collection.UpdateOneAsync(filter, update, updateOptions);
    }

    public async Task StoreFribbItems(IFormFile formFile)
    {
        try
        {
            using var stream = formFile.OpenReadStream();

            var items = await JsonSerializer.DeserializeAsync<List<FribbAniListItem>>(stream);

            var models = new List<WriteModel<FribbAniListItem>>();

            if (items is null)
                throw new InvalidCastException("Failed to deserialize file.");

            foreach (var doc in items)
            {
                var filter = Builders<FribbAniListItem>.Filter.Eq(d => d.AniListId, doc.AniListId);
                var model = new ReplaceOneModel<FribbAniListItem>(filter, doc) { IsUpsert = true };
                models.Add(model);
            }
            var collection = _database.GetCollection<FribbAniListItem>("fribbList");
            await collection.BulkWriteAsync(models);

        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, ex.Message);
            throw;
        }
    }

    public async Task<List<WatchlistItem>> GetUpdatedWatchlistEntries()
    {
        var config = await GetConfigAsync();
        var watchlistEntries = await _aniService.GetUserWatchListAsync(config.UserName);
        List<AniListItem> aniListItems = [.. watchlistEntries.Data.MediaListCollection.Lists
            .SelectMany(x => x.Entries)
            .Select(x => new AniListItem() { AniListId = x.Media.Id, Title = x.Media.Title.English })];

        Dictionary<int, WatchlistItem> watchlistDictionary = new();

        var fribbCollection = _database.GetCollection<FribbAniListItem>("fribbList");
        foreach (var item in aniListItems)
        {
            var fribbItem = await fribbCollection.Find(x => x.AniListId == item.AniListId).FirstOrDefaultAsync();
            var tvdb = fribbItem?.TvdbId ?? 0;

            if (watchlistDictionary.TryGetValue(tvdb, out var watchlistItem))
            {
                watchlistItem.AniListItems.Add(item);
            }
            else
            {
                watchlistItem = new WatchlistItem()
                {
                    AniListItems = [item],
                    Title = item.Title,
                    TvdbId = tvdb
                };
                watchlistDictionary[tvdb] = watchlistItem;
            }
        }

        var existingWatchlistCollection = _database.GetCollection<WatchlistItem>("watchlistItem");

        foreach (var item in watchlistDictionary)
        {
            var existingWatchlistItem = await existingWatchlistCollection.Find(x => x.TvdbId == item.Key).FirstOrDefaultAsync();
            if (existingWatchlistItem is null) continue;
            item.Value.AniListItems.ExceptWith(existingWatchlistItem.AniListItems);
        }

        return [.. watchlistDictionary.Values];
    }
    public async Task SaveWatchlistEntries(List<WatchlistItem> watchlistItems)
    {
        var models = new List<WriteModel<WatchlistItem>>();
        foreach (var doc in watchlistItems)
        {
            var filter = Builders<WatchlistItem>.Filter.Eq(d => d.TvdbId, doc.TvdbId);
            var model = new ReplaceOneModel<WatchlistItem>(filter, doc) { IsUpsert = true };
            models.Add(model);
        }

        var existingWatchlistCollection = _database.GetCollection<WatchlistItem>("watchlistItem");
        await existingWatchlistCollection.BulkWriteAsync(models);
    }

}

