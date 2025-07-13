using SyncSenpai.Ani.Entities.GraphQLWatchList;
using System.Text;
using System.Text.Json;

namespace SyncSenpai.Server.Services;

public partial class AniService
{
    private readonly ILogger<AniService> _logger;
    private readonly HttpClient _httpClient;
    private const string _WatchListQuery = @"query User($userName: String, $type: MediaType) 
                { MediaListCollection( userName: $userName, type: $type) 
                { lists { name entries { media { id title { english } } } status } }}";

    private const string _aniListEndpoint = "https://graphql.anilist.co";

    public AniService(ILogger<AniService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
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

    
}
