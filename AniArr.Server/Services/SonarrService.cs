using AniArr.Server.Entities.Sonarr;
using MongoDB.Driver;
using System.Text;
using System.Text.Json;

namespace AniArr.Server.Services;

public partial class SonarrService(ILogger<SonarrService> logger, HttpClient httpClient, MongoDbService mongoDbService)
{
    private void SetupClient(SonarrConnectionDetails connectionDetails)
    {
        httpClient.BaseAddress = new($"{connectionDetails.Host}:{connectionDetails.Port}");
        httpClient.DefaultRequestHeaders.Add("X-Api-Key", connectionDetails.ApiKey);
    }
    #region Logging
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "{methodName}")]
    partial void LogMethod([System.Runtime.CompilerServices.CallerMemberName] string methodName = "");

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "{methodName}: {lookupTitle}")]
    partial void LogMethodWithTitle(string lookupTitle, [System.Runtime.CompilerServices.CallerMemberName] string methodName = "");

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "{methodName}: {tvDbId}")]
    partial void LogMethodWithTvDbId(int tvDbId, [System.Runtime.CompilerServices.CallerMemberName] string methodName = "");

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "{methodName}: {requestJson}")]
    partial void LogMethodRequest(string requestJson, [System.Runtime.CompilerServices.CallerMemberName] string methodName = "");

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "{methodName}: Error communicating with Sonarr")]
    partial void LogException(Exception ex, [System.Runtime.CompilerServices.CallerMemberName] string methodName = "");
    #endregion

    public async Task<bool> UpdateConnectionDetails(SonarrConnectionDetails sonarrConnectionDetails)
    {
        LogMethod();
        SetupClient(sonarrConnectionDetails);
        var result = await httpClient.GetAsync("/api");
        if (!result.IsSuccessStatusCode) return false;

        SonarrConfig config = new();
        config.SonarrConnectionDetails = sonarrConnectionDetails;

        var filter = Builders<SonarrConfig>.Filter.Eq(x => x.Id, nameof(SonarrConfig));
        var update = Builders<SonarrConfig>.Update.Set(x => x.SonarrConnectionDetails, sonarrConnectionDetails);
        UpdateOptions updateOptions = new() { IsUpsert = true };

        var collection = mongoDbService.GetCollection<SonarrConfig>(nameof(SonarrConfig));

        await collection.UpdateOneAsync(filter, update, updateOptions);

        return true;
    }
    public async Task<SonarrConfig> LoadConfigFromSonarr()
    {
        LogMethod();
        try
        {
            var collection = mongoDbService.GetCollection<SonarrConfig>(nameof(SonarrConfig));
            var sonarrConfig = (await collection.FindAsync(x => x.Id == nameof(SonarrConfig))).FirstOrDefault();

            SetupClient(sonarrConfig.SonarrConnectionDetails);
            sonarrConfig.SonarrTags = await GetSonarrTags();
            sonarrConfig.QualityProfiles = await GetQualityProfiles();
            sonarrConfig.RootFolders = await GetRootFolders();
            return sonarrConfig;
        }
        catch (Exception ex)
        {
            LogException(ex);
            throw;
        }
    }
    private async Task<List<SonarrConfig.SonarrTag>> GetSonarrTags()
    {
        LogMethod();
        var tags = await httpClient.GetAsync("/api/v3/tag");
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
    private async Task<List<SonarrConfig.QualityProfile>> GetQualityProfiles()
    {
        LogMethod();
        var response = await httpClient.GetAsync("/api/v3/qualityprofile");

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
    private async Task<List<SonarrConfig.RootFolder>> GetRootFolders()
    {
        LogMethod();
        var response = await httpClient.GetAsync("/api/v3/rootfolder");

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
        LogMethod();
        var filter = Builders<SonarrConfig>.Filter.Eq(x => x.Id, nameof(SonarrConfig));

        var collection = mongoDbService.GetCollection<SonarrConfig>(nameof(SonarrConfig));
        var replaceOptions = new ReplaceOptions { IsUpsert = true };

        var result = await collection.ReplaceOneAsync(filter, sonarrConfig, replaceOptions);
    }

    public async Task<SonarrConfig> GetSonarrConfig()
    {
        LogMethod();
        var collection = mongoDbService.GetCollection<SonarrConfig>(nameof(SonarrConfig));
        var config = await collection.Find(x => x.Id == nameof(SonarrConfig)).FirstOrDefaultAsync();
        return config ?? new();
    }
    public async Task<SonarrLookup> LookupGetByTitle(string lookupTitle)
    {
        LogMethodWithTitle(lookupTitle);
        var sonarrConfig = await GetSonarrConfig();
        SetupClient(sonarrConfig.SonarrConnectionDetails);

        var response = await httpClient.GetAsync($"/api/v3/series/lookup?term={lookupTitle}");
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
    public async Task<SonarrLookup> LookupGetByTvDbId(int tvdbId)
    {
        LogMethodWithTvDbId(tvdbId);
        var sonarrConfig = await GetSonarrConfig();
        SetupClient(sonarrConfig.SonarrConnectionDetails);

        var response = await httpClient.GetAsync($"/api/v3/series/lookup?term=tvdb:{tvdbId}&includeSeasonImages=false");
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
    public async Task<SonarrLookup?> SeriesGetByTvDbId(int tvdbId)
    {
        LogMethodWithTvDbId(tvdbId);
        var sonarrConfig = await GetSonarrConfig();
        SetupClient(sonarrConfig.SonarrConnectionDetails);

        var response = await httpClient.GetAsync($"/api/v3/series?tvdbId={tvdbId}&includeSeasonImages=false");
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStreamAsync();
            var deserialized = await JsonSerializer.DeserializeAsync<List<SonarrLookup>>(responseContent);
            if (deserialized is not null)
            {
                return deserialized.FirstOrDefault();
            }
        }
        throw new InvalidDataException("Failed to lookup series");
    }

    internal async Task RequestSeries(SonarrRequest sonarrRequest)
    {
        LogMethodRequest(JsonSerializer.Serialize(sonarrRequest));
        var sonarrConfig = await GetSonarrConfig();
        SetupClient(sonarrConfig.SonarrConnectionDetails);

        var json = JsonSerializer.Serialize(sonarrRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("/api/v3/series", content);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidDataException("Failed to lookup series");
        }
    }
}
