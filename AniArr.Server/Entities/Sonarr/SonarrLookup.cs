using System.Text.Json.Serialization;

namespace AniArr.Server.Entities.Sonarr;

public class SonarrLookup: SonarrSeriesBase
{
    public int year { get; set; }
    public string path { get; set; }
    public int id { get; set; }
    public bool seasonFolder { get; set; }
    public bool monitored { get; set; }
    public string folder { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";
}
