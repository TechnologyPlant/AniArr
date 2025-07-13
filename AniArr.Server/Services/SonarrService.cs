using AniArr.Server.Entities;
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

    public async Task<bool> TestConnection(SonarrConnectionDetails sonarrConfig)
    {
        SetupClient(sonarrConfig);
        var result = await _httpClient.GetAsync("/api");
        return result.IsSuccessStatusCode;
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
        await _mongoDbService.SaveSonarrConfig(sonarrConfig);
    }

    public async Task<SonarrConfig> GetSonarrConfig()
    {
        return await _mongoDbService.GetSonarrConfig();
    }
}
