using MongoDB.Bson.Serialization.Attributes;

namespace AniArr.Server.Entities;

public class AnilistConfigModel
{
    [BsonId]
    public string Id { get; init; } = "anilistConfigModel";

    [BsonElement("userName")]
    public string UserName { get; set; } = "";
}
