using AniArr.Server.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AniArr.Server.Services;
public class MongoDbService
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<MongoDbService> _logger;

    public MongoDbService(IOptions<MongoSettings> mongoOptions, ILogger<MongoDbService> logger)
    {
        var settings = mongoOptions.Value;

        var credentials = string.IsNullOrWhiteSpace(settings.Username)
            ? ""
            : $"{settings.Username}:{settings.Password}@";

        var connectionString = $"mongodb://{credentials}{settings.Host}:{settings.Port}";
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(settings.Database);
        _logger = logger;
    }

    public IMongoCollection<T> GetCollection<T>(string name) =>
        _database.GetCollection<T>(name);

}

