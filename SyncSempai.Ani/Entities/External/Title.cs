using System.Text.Json.Serialization;

namespace SyncSempai.Ani.Entities.External;

public class Title
{
    [JsonPropertyName("english")]
    public string English { get; set; } = "";
}
