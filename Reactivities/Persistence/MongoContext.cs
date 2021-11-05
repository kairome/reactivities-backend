using AspNetCore.ServiceRegistration.Dynamic;
using MongoDB.Driver;

namespace Persistence
{
    [ScopedService]
    public interface IMongoContext
    {
        IMongoClient Client { get; }
    }

    public class MongoContext : IMongoContext
    {
        private readonly IMongoClient _mongoClient;
        public IMongoClient Client => _mongoClient;

        public MongoContext(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }
    }
}