using AniArr.Server.Entities;
using AniArr.Server.Entities.GraphQLWatchList;
using MongoDB.Driver;
using System.Text;
using System.Text.Json;

namespace AniArr.Server.Services;

public partial class AniService
{
    private readonly ILogger<AniService> _logger;
    private readonly HttpClient _httpClient;
    private const string _WatchListQuery = @"query User($userName: String, $type: MediaType) 
                { MediaListCollection( userName: $userName, type: $type) 
                { lists { name entries { media { id title { english } } } status } }}";

    private const string _aniListEndpoint = "https://graphql.anilist.co";
    private readonly MongoDbService _mongoDbService;

    public AniService(ILogger<AniService> logger, HttpClient httpClient, MongoDbService mongoDbService)
    {
        _logger = logger;
        _httpClient = httpClient;
        _mongoDbService = mongoDbService;
    }


    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "{methodName}:{username}")]
    partial void LogMethodWithUsername(string username, [System.Runtime.CompilerServices.CallerMemberName] string methodName = "");

    public async Task<Root> GetUserWatchListAsync(string username, CancellationToken cancellationToken)
    {
        LogMethodWithUsername(username);

        var queryObject = new
        {
            query = _WatchListQuery,
            variables = new { userName = username, type = "ANIME" }
        };
        StringContent content = new(JsonSerializer.Serialize(queryObject), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_aniListEndpoint, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        Root? result;
        try
        {
            var responseContent = await response.Content.ReadAsStreamAsync(cancellationToken);
            result = await JsonSerializer.DeserializeAsync<Root>(responseContent, cancellationToken: cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }

        if (result is null)
        {
            throw new InvalidOperationException();
        }
        return result;
    }

    public async Task<AnilistConfigModel> GetConfigAsync(CancellationToken cancellationToken)
    {
        var collection = _mongoDbService.GetCollection<AnilistConfigModel>(nameof(AnilistConfigModel));
        var config = await collection.Find(x => x.Id == nameof(AnilistConfigModel)).FirstOrDefaultAsync(cancellationToken);
        return config ?? new();
    }

    public async Task SetAniListUserNameAsync(string username, CancellationToken cancellationToken)
    {
        var filter = Builders<AnilistConfigModel>.Filter.Eq(x => x.Id, nameof(AnilistConfigModel));

        var collection = _mongoDbService.GetCollection<AnilistConfigModel>(nameof(AnilistConfigModel));
        var update = Builders<AnilistConfigModel>.Update
            .Set(x => x.UserName, username);
        var updateOptions = new UpdateOptions { IsUpsert = true };

        var result = await collection.UpdateOneAsync(filter, update, updateOptions, cancellationToken);
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
            var collection = _mongoDbService.GetCollection<FribbAniListItem>("fribbList");
            await collection.BulkWriteAsync(models);

        }
        catch (Exception ex)
        {
            _logger.LogError(0, ex, ex.Message);
            throw;
        }
    }

    public async Task<List<WatchlistItem>> GetUpdatedWatchlistEntries(CancellationToken cancellationToken)
    {
        var config = await GetConfigAsync(cancellationToken);
        var watchlistEntries = await GetUserWatchListAsync(config.UserName, cancellationToken);
        List<AniListItem> aniListItems = [.. watchlistEntries.Data.MediaListCollection.Lists
            .SelectMany(x => x.Entries)
            .Select(x => new AniListItem() { AniListId = x.Media.Id, Title = x.Media.Title.English })];

        Dictionary<int, WatchlistItem> watchlistDictionary = new();

        var fribbCollection = _mongoDbService.GetCollection<FribbAniListItem>("fribbList");
        foreach (var item in aniListItems)
        {
            var fribbItem = await fribbCollection.Find(x => x.AniListId == item.AniListId).FirstOrDefaultAsync(cancellationToken);
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

        var existingWatchlistCollection = _mongoDbService.GetCollection<WatchlistItem>(nameof(WatchlistItem));

        foreach (var item in watchlistDictionary)
        {
            var existingWatchlistItem = await existingWatchlistCollection.Find(x => x.TvdbId == item.Key).FirstOrDefaultAsync(cancellationToken);
            if (existingWatchlistItem is null) continue;
            item.Value.AniListItems.ExceptWith(existingWatchlistItem.AniListItems);
        }

        return [.. watchlistDictionary.Values.Where(x => x.AniListItems.Count > 0)];
    }

    public IQueryable<WatchlistItem> GetWatchlistEntries()
    {
        var collection = _mongoDbService.GetCollection<WatchlistItem>(nameof(WatchlistItem));

        return collection.AsQueryable();
    }
    public async Task SaveWatchlistItem(WatchlistItem watchlistItem)
    {
        var models = new List<WriteModel<WatchlistItem>>();
        var watchlistCollection = _mongoDbService.GetCollection<WatchlistItem>(nameof(WatchlistItem));

        var existing = await watchlistCollection.Find(x => x.TvdbId == watchlistItem.TvdbId).FirstOrDefaultAsync();

        if (existing is null)
            existing = watchlistItem;
        else
        {
            existing.AniListItems.UnionWith(watchlistItem.AniListItems);
        }

        var update = Builders<WatchlistItem>.Update
            .PushEach(x => x.AniListItems, watchlistItem.AniListItems);
        var updateOptions = new UpdateOptions { IsUpsert = true };
        var filter = Builders<WatchlistItem>
            .Filter.Eq(d => d.TvdbId, watchlistItem.TvdbId);

        var result = await watchlistCollection.UpdateOneAsync(filter, update, updateOptions);
    }

    public async Task DeleteAllWatchListItem(CancellationToken cancellationToken)
    {
        var watchlistCollection = _mongoDbService.GetCollection<WatchlistItem>(nameof(WatchlistItem));

        var filter = Builders<WatchlistItem>.Filter.Empty;
        await watchlistCollection.DeleteManyAsync(filter, cancellationToken);
    }

}
