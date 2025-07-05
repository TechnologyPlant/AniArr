using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SyncSenpai.Server.Entities;

namespace SyncSenpai.Server.Services;
public class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(IOptions<MongoSettings> mongoOptions)
    {
        var settings = mongoOptions.Value;

        var credentials = string.IsNullOrWhiteSpace(settings.Username)
            ? ""
            : $"{settings.Username}:{settings.Password}@";

        var connectionString = $"mongodb://{credentials}{settings.Host}:{settings.Port}";
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(settings.Database);
    }

    public async Task<AnilistConfigModel> GetConfigAsync()
    {
        var collection = _database.GetCollection<AnilistConfigModel>("config");
        var config = await collection.Find(x => x.Id == "serviceConfig").FirstOrDefaultAsync();
        return config ?? new();
    }

    public async Task SetAniListUserNameAsync(string username)
    {
        var filter = Builders<AnilistConfigModel>.Filter.Eq(x => x.Id, "serviceConfig");

        var collection = _database.GetCollection<AnilistConfigModel>("config");
        var update = Builders<AnilistConfigModel>.Update
            .Set(x => x.UserName, username);
        var updateOptions = new UpdateOptions { IsUpsert = true };

        var result = await collection.UpdateOneAsync(filter, update, updateOptions);
    }
}

