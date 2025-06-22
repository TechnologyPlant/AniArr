using System.Text.Json.Serialization;

namespace SyncSenpai.Sonarr.Entities;

public class SonarrRequest
{
    public int TvDbId { get; set; }
    public int QualityProfileId { get; set; }
    public int RootFolderPath { get; set; }

    public string ToPostRequestBody()
    {
        return $"{{" +
            $"\"tvdbId\":{TvDbId}," +
            $"\"qualityProfileId\":{QualityProfileId}," +
            $"\"rootFolderPath\":{RootFolderPath}," +
            $"\"monitored\":true," +
            $"  \"addOptions\":{{ \"searchForMissingEpisodes\":true }}" +
            $"}}";
    }
}

public class LookupResponse
{
    [JsonPropertyName("tvdbId")]
    public int TvDbId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("seasons")]
    public List<Season> Seasons { get; set; }

    public class Season
    {
        [JsonPropertyName("seasonNumber")]
        public int SeasonNumber { get; set; }
    }
}
