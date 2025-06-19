using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SyncSenpai.Sonarr.Entities;

public class SonarrTag:IEquatable<SonarrTag>
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("label")]
    public string Name { get; set; } = "";

    public bool Equals(SonarrTag? other)
    {
        return Equals(this?.Id, other?.Id);
    }
}