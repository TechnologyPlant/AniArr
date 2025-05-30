using System.Text.Json.Serialization;

namespace SyncSenpai.Server.Entities.External;

public class Title
{
    [JsonPropertyName("english")]
    public string English { get; set; } = "";
}
