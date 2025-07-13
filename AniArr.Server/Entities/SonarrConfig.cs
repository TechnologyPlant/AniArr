using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace AniArr.Server.Entities;

public class SonarrConfig
{
    [BsonId]
    public string Id { get; init; } = nameof(SonarrConfig);

    [BsonElement("connectionDetails")]
    public SonarrConnectionDetails SonarrConnectionDetails { get; set; } = new();

    [BsonElement("activeSonarrTag")]
    public SonarrTag ActiveSonarrTag { get; set; } = new() { Id = -1, Name = "Select a tag" };
    [BsonElement("sonarrTags")]
    public List<SonarrTag> SonarrTags { get; set; } = [];

    [BsonElement("activeQualityProfile")]
    public QualityProfile ActiveQualityProfile { get; set; } = new() { Id = -1, Name = "Select a Quality Profile" };
    [BsonElement("qualityProfiles")]
    public List<QualityProfile> QualityProfiles { get; set; } = [];

    [BsonElement("activeRootFolder")]
    public RootFolder ActiveRootFolder { get; set; } = new() { Id = -1, Name = "Select a Root Folder" };
    [BsonElement("rootFolders")]
    public List<RootFolder> RootFolders { get; set; } = [];

    public record SonarrTag
    {
        [JsonPropertyName("id")]
        [BsonElement("id")]
        public int Id { get; set; }

        [JsonPropertyName("label")]
        [BsonElement("name")]
        public string Name { get; set; } = "";
    }
    public record QualityProfile
    {
        [JsonPropertyName("id")]
        [BsonElement("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        [BsonElement("name")]
        public string Name { get; set; } = "";
    }
    public record RootFolder
    {
        [JsonPropertyName("id")]
        [BsonElement("id")]
        public int Id { get; set; }

        [JsonPropertyName("path")]
        [BsonElement("name")]
        public string Name { get; set; } = "";
    }

}
public record SonarrConnectionDetails
{
    [BsonElement("host")]
    public string Host { get; set; } = "";
    [BsonElement("port")]
    public string Port { get; set; } = "";
    [BsonElement("apiKey")]
    public string ApiKey { get; set; } = "";
}

