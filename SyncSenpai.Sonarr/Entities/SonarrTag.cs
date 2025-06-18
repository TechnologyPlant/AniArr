using Newtonsoft.Json;

namespace SyncSenpai.Sonarr.Entities;

public class SonarrTag : IEquatable<SonarrTag>
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("label")]
    public string Name { get; set; } = "";

    public bool Equals(SonarrTag? other)
    {
        return Equals(this?.Id, other?.Id);
    }
}