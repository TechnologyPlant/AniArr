using System.Text.Json.Serialization;

namespace SyncSenpai.Server.Entities;

public class SonarrConfig
{
    public int Id { get; init; } = 1;
    public ConnectionDetails ConnectionDetails { get; set; } = new(); 

    public SonarrTag ActiveSonarrTag { get; set; } = new() { Id = -1, Name = "Select a tag" };
    public List<SonarrTag> SonarrTags { get; set; } = [];

    public QualityProfile ActiveQualityProfile { get; set; } = new() { Id = -1, Name = "Select a Quality Profile" };
    public List<QualityProfile> QualityProfiles { get; set; } = [];

    public RootFolder ActiveRootFolder { get; set; } = new() { Id = -1, Name = "Select a Root Folder" };
    public List<RootFolder> RootFolders { get; set; } = [];

    public record SonarrTag
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("label")]
        public string Name { get; set; } = "";
    }
    public record QualityProfile
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }
    public record RootFolder
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("path")]
        public string Name { get; set; } = "";
    }

}
    public record SonarrConnectionDetails
    {
    public string Host { get; set; } = "";
    public string Port { get; set; } = "";
    public string ApiKey { get; set; } = "";
}

