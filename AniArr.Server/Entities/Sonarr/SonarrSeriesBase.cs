using System.Text.Json.Serialization;

namespace AniArr.Server.Entities.Sonarr;

public abstract class SonarrSeriesBase
{
    [JsonPropertyName("title")]
    public string title { get; set; }
    [JsonPropertyName("tvdbId")]
    public int tvdbId { get; set; }
    [JsonPropertyName("seasons")]
    public List<Season> seasons { get; set; } = [];

    public class Season
    {
        [JsonPropertyName("seasonNumber")]
        public int seasonNumber { get; set; }
        [JsonPropertyName("monitored")]
        public bool monitored { get; set; }
    }
}
