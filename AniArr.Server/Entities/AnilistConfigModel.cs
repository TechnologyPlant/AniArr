using MongoDB.Bson.Serialization.Attributes;

namespace AniArr.Server.Entities;

public class AnilistConfigModel
{
    [BsonId]
    public string Id { get; init; } = nameof(AnilistConfigModel);

    [BsonElement("userName")]
    public string UserName { get; set; } = "";
}
