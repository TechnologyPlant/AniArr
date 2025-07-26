using AniArr.Server.Entities.Sonarr;
using MongoDB.Driver;
using System.Text;
using System.Text.Json;

namespace AniArr.Server.Services;

public class SonarrService
{
    private readonly ILogger<SonarrService> _logger;
    private readonly HttpClient _httpClient;
    private readonly MongoDbService _mongoDbService;

    public SonarrService(ILogger<SonarrService> logger, HttpClient httpClient, MongoDbService mongoDbService)
    {
        _logger = logger;
        _httpClient = httpClient;
        _mongoDbService = mongoDbService;
    }

    private void SetupClient(SonarrConnectionDetails sonarrConfig, bool overrideConfiguration = false)
    {
        if (_httpClient.BaseAddress is null || overrideConfiguration)
        {
            _httpClient.BaseAddress = new($"{sonarrConfig.Host}:{sonarrConfig.Port}");
            _httpClient.DefaultRequestHeaders.Add("X-Api-Key", sonarrConfig.ApiKey);
        }
    }

    public async Task<bool> UpdateConnectionDetails(SonarrConnectionDetails sonarrConnectionDetails)
    {
        SetupClient(sonarrConnectionDetails);
        var result = await _httpClient.GetAsync("/api");
        if (!result.IsSuccessStatusCode) return false;

        SonarrConfig config = new();
        config.SonarrConnectionDetails = sonarrConnectionDetails;

        var filter = Builders<SonarrConfig>.Filter.Eq(x => x.Id, nameof(SonarrConfig));
        var update = Builders<SonarrConfig>.Update.Set(x => x.SonarrConnectionDetails, sonarrConnectionDetails);

        var collection = _mongoDbService.GetCollection<SonarrConfig>(nameof(SonarrConfig));

        await collection.UpdateOneAsync(filter, update);

        return true;
    }
    public async Task<List<SonarrConfig.SonarrTag>> GetSonarrTags(SonarrConnectionDetails sonarrConfig)
    {
        SetupClient(sonarrConfig);
        var tags = await _httpClient.GetAsync("/api/v3/tag");
        if (tags.IsSuccessStatusCode)
        {
            var contentStream = await tags.Content.ReadAsStreamAsync();
            var deserializedTags = await JsonSerializer.DeserializeAsync<List<SonarrConfig.SonarrTag>>(contentStream);
            if (deserializedTags is not null)
            {
                return deserializedTags;
            }
        }

        throw new InvalidDataException("No Sonarr Tags to load");
    }
    public async Task<List<SonarrConfig.QualityProfile>> LoadQualityProfiles(SonarrConnectionDetails sonarrConfig)
    {
        SetupClient(sonarrConfig);
        var response = await _httpClient.GetAsync("/api/v3/qualityprofile");

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStreamAsync();
            var deserialized = await JsonSerializer.DeserializeAsync<List<SonarrConfig.QualityProfile>>(responseContent);
            if (deserialized is not null && deserialized.Count > 0)
            {
                return deserialized;
            }
        }
        throw new InvalidDataException("No Sonarr Quality Profiles to load");
    }
    public async Task<List<SonarrConfig.RootFolder>> LoadRootFolders(SonarrConnectionDetails sonarrConfig)
    {
        SetupClient(sonarrConfig);
        var response = await _httpClient.GetAsync("/api/v3/rootfolder");

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStreamAsync();
            var deserialized = await JsonSerializer.DeserializeAsync<List<SonarrConfig.RootFolder>>(responseContent);
            if (deserialized is not null && deserialized.Count > 0)
            {
                return deserialized;
            }
        }
        throw new InvalidDataException("No Sonarr Root Folders to load");
    }
    public async Task SaveSonarrConfig(SonarrConfig sonarrConfig)
    {
        var filter = Builders<SonarrConfig>.Filter.Eq(x => x.Id, nameof(SonarrConfig));

        var collection = _mongoDbService.GetCollection<SonarrConfig>(nameof(SonarrConfig));
        var replaceOptions = new ReplaceOptions { IsUpsert = true };

        var result = await collection.ReplaceOneAsync(filter, sonarrConfig, replaceOptions);
    }

    public async Task<SonarrConfig> GetSonarrConfig()
    {
        var collection = _mongoDbService.GetCollection<SonarrConfig>(nameof(SonarrConfig));
        var config = await collection.Find(x => x.Id == nameof(SonarrConfig)).FirstOrDefaultAsync();
        return config ?? new();
    }
    public async Task<SonarrLookup> LookupSeries(string lookupTitle)
    {
        var sonarrConfig = await GetSonarrConfig();
        SetupClient(sonarrConfig.SonarrConnectionDetails);

        var response = await _httpClient.GetAsync($"/api/v3/series/lookup?term={lookupTitle}");
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStreamAsync();
            var deserialized = await JsonSerializer.DeserializeAsync<List<SonarrLookup>>(responseContent);
            if (deserialized is not null && deserialized.Count > 0)
            {
                return deserialized.First();
            }
        }
        throw new InvalidDataException("Failed to lookup series");

    }

    internal async Task RequestSeries(SonarrRequest sonarrRequest)
    {
        var sonarrConfig = await GetSonarrConfig();
        SetupClient(sonarrConfig.SonarrConnectionDetails);

        var json = JsonSerializer.Serialize(sonarrRequest.ToPostRequestBody());
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/v3/series", content);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException("Failed to lookup series");
        }
    }
}
