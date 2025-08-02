using System.Text.Json.Serialization;

namespace AniArr.Server.Entities.Sonarr;

public class SonarrRequest : SonarrSeriesBase
{
    [JsonPropertyName("qualityProfileId")]
    public int QualityProfileId { get; set; }

    [JsonPropertyName("rootFolderPath")]
    public string RootFolderPath { get; set; }

    [JsonPropertyName("seriesType")]
    public string SeriesType { get; set; }

    /// <summary>
    /// Outdated property, does not need to be set by user
    /// </summary>
    [JsonPropertyName("languageProfileId")]
    public int LanguageProfileId { get; set; } = 1;

}