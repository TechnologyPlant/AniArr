using AniArr.Server.Entities;
using AniArr.Server.Entities.GraphQLWatchList;
using MongoDB.Bson;
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

    public async Task GetUpdatedWatchlistEntries(CancellationToken cancellationToken)
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

        var models = new List<WriteModel<WatchlistItem>>();
        foreach (var watchlistItem in watchlistDictionary.Values)
        {
            var aniListIds = watchlistItem.AniListItems.Select(a => a.AniListId).ToList();

            var filter = Builders<WatchlistItem>.Filter.And(
                Builders<WatchlistItem>.Filter.Eq(w => w.TvdbId, watchlistItem.TvdbId),
                Builders<WatchlistItem>.Filter.Not(
                    Builders<WatchlistItem>.Filter.ElemMatch(
                        w => w.AniListItems,
                        Builders<AniListItem>.Filter.In(a => a.AniListId, aniListIds)
                    )
                )
            );

            var update = Builders<WatchlistItem>.Update.PushEach(
                w => w.AniListItems,
                watchlistItem.AniListItems
            )
                .Set(w => w.Title, watchlistItem.Title);

            models.Add(new UpdateOneModel<WatchlistItem>(filter, update)
            {
                IsUpsert = true
            });
        }

        var watchlistCollection = _mongoDbService.GetCollection<WatchlistItem>(nameof(WatchlistItem));
        await watchlistCollection.BulkWriteAsync(models);
    }

    public IQueryable<WatchlistItem> GetWatchlistEntries()
    {
        var collection = _mongoDbService.GetCollection<WatchlistItem>(nameof(WatchlistItem));
        return collection.AsQueryable();
    }

    public async Task DeleteAllWatchListItem(CancellationToken cancellationToken)
    {
        var watchlistCollection = _mongoDbService.GetCollection<WatchlistItem>(nameof(WatchlistItem));

        var filter = Builders<WatchlistItem>.Filter.Empty;
        await watchlistCollection.DeleteManyAsync(filter, cancellationToken);
    }

    internal async Task PutWatchlistItem(WatchlistItem watchlistItem, CancellationToken cancellationToken)
    {
        var filter = Builders<WatchlistItem>.Filter.Eq(d => d.TvdbId, watchlistItem.TvdbId);
        var replaceOptions = new ReplaceOptions() { IsUpsert = true };
        var collection = _mongoDbService.GetCollection<WatchlistItem>(nameof(WatchlistItem));
        await collection.ReplaceOneAsync(filter, watchlistItem, replaceOptions, cancellationToken);
    }
}
