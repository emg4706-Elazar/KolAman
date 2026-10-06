

using ControlSystem.Models;

namespace ControlSystem.Repositories;

public interface IMongoRepository
{
    Task SaveToMongo
        (string collectionName,
        Alert alert);
}
