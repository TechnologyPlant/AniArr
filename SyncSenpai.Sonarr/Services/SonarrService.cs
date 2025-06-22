using Microsoft.Extensions.Logging;
using SyncSenpai.Sonarr.Entities;
using SyncSenpai.Sonarr.Repositories;
using System.Text.Json;

namespace SyncSenpai.Sonarr.Services;

public class SonarrService
{
    private readonly ILogger<SonarrService> _logger;
    private readonly HttpClient _httpClient;
    private readonly SonarrConfigRepository _sonarrConfigRepository;

    public SonarrService(ILogger<SonarrService> logger, HttpClient httpClient, SonarrConfigRepository sonarrConfigRepository)
    {
        _logger = logger;
        _httpClient = httpClient;
        _sonarrConfigRepository = sonarrConfigRepository;
    }

    public void SetupClient(SonarrConfig sonarrConfig)
    {
        _httpClient.BaseAddress = new($"{sonarrConfig.SonarrUrl}");
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", sonarrConfig.SonarrApiKey);
    }

    public async Task<List<SonarrConfig.SonarrTag>?> GetSonarrTags()
    {
        var sonarrConfig = await _sonarrConfigRepository.GetConfigAsync();

        var tags = await _httpClient.GetAsync("/api/v3/tag");
        if (tags.IsSuccessStatusCode)
        {
            var contentStream = await tags.Content.ReadAsStreamAsync();
            var deserializedTags = await JsonSerializer.DeserializeAsync<List<SonarrConfig.SonarrTag>>(contentStream);
            if (deserializedTags is null)
            {
                return null;
            }
            else
            {
                return deserializedTags;
            }
        }
        return null;
    }

    public async Task<LookupResponse?> LookupSeriesAsync(int tvDbId)
    {
        var sonarrConfig = await _sonarrConfigRepository.GetConfigAsync();
        SetupClient(sonarrConfig);

        var response = await _httpClient.GetAsync($"/api/v3/series/lookup?term=tvdb:{tvDbId}");

        if (response.IsSuccessStatusCode)
        {
            var contentStream = await response.Content.ReadAsStreamAsync();
            var deserialized = await JsonSerializer.DeserializeAsync<LookupResponse>(contentStream);
            if (deserialized is null)
            {
                return null;
            }
            else
            {
                return deserialized;
            }
        }

        return null;
    }
}
