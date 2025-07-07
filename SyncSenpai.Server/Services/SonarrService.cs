using Blazored.Toast.Services;
using Microsoft.Extensions.Logging;
using SyncSenpai.Server.Entities;
using System.Text;
using System.Text.Json;

namespace SyncSenpai.Server.Services;

public class SonarrService
{
    private readonly ILogger<SonarrService> _logger;
    private readonly HttpClient _httpClient;

    public SonarrService(ILogger<SonarrService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    private void SetupClient(SonarrConfig sonarrConfig)
    {
        _httpClient.BaseAddress = new($"{sonarrConfig.Host}:{sonarrConfig.Port}");
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", sonarrConfig.ApiKey);
    }

    public async Task<bool> TestConnection(SonarrConfig sonarrConfig)
    {
        SetupClient(sonarrConfig);
        var result = await _httpClient.GetAsync("/api");
        return result.IsSuccessStatusCode;
    }
}
