using Microsoft.Extensions.Logging;
using SyncSenpai.Ani.Entities;
using SyncSenpai.Ani.Entities.GraphQLWatchList;
using SyncSenpai.Ani.Repositories;
using System.Text;
using System.Text.Json;

namespace SyncSenpai.Ani.Services;

public partial class AniService : IAniService
{
    private readonly ILogger<AniService> _logger;
    private readonly HttpClient _httpClient;
    private readonly WatchListRepository _watchListRepository;
    private readonly IConfigRepository _configRepository;
    private const string _WatchListQuery = @"query User($userName: String, $type: MediaType) 
                { MediaListCollection( userName: $userName, type: $type) 
                { lists { name entries { media { id title { english } } } status } }}";

    private const string _aniListEndpoint = "https://graphql.anilist.co";

    public AniService(ILogger<AniService> logger, HttpClient httpClient, WatchListRepository watchListRepository, IConfigRepository configRepository)
    {
        _logger = logger;
        _httpClient = httpClient;
        _watchListRepository = watchListRepository;
        _configRepository = configRepository;
    }
    public async Task<ConfigModel> GetConfigAsync()
    {
        return await _configRepository.GetConfigAsync();
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "{methodName}:{username}")]
    partial void LogMethodWithUsername(string username, [System.Runtime.CompilerServices.CallerMemberName] string methodName = "");

    public async Task<Root> GetUserWatchListAsync(string username)
    {
        LogMethodWithUsername(username);

        var queryObject = new
        {
            query = _WatchListQuery,
            variables = new { userName = username, type = "ANIME" }
        };
        StringContent content = new(JsonSerializer.Serialize(queryObject), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_aniListEndpoint, content);
        response.EnsureSuccessStatusCode();
        Root? result;
        try
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<Root>(responseContent);
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

    public async Task StoreAniUserAsync(AniUser aniUser)
    {
        await _watchListRepository.StoreAniUserAsync(aniUser);
    }

    public async Task<int> GetTvDbIdByAniListId(int aniListId)
    {
        return await _configRepository.GetTvDbIdByAniListId(aniListId);
    }

    public async Task<List<WatchListEntry>> GetPendingEntriesAsync()
    {
        return await _watchListRepository.GetPendingEntriesAsync();
    }

}
