using MongoDB.Driver;
using ControlSystem.Models;


namespace ControlSystem.Repositories;

public class MongoRepository : IMongoRepository
{
    private readonly IMongoDatabase _database;

    public MongoRepository(
        IMongoDatabase database)
    {
        _database = database;
    }

    public async Task SaveToMongo
        (string collectionName,
        Alert alert)
    {
        var collection = _database
            .GetCollection<Alert>(
            collectionName);


        await collection.InsertOneAsync(alert);
    }

}
