using System.Threading.Tasks;
using Domain;
using MongoDB.Driver;

namespace Persistence
{
    public interface IWriteDbOperationsService<TDocument> where TDocument : IDDocument 
    {
        Task<TDocument> InsertOne(TDocument doc);

        Task<TDocument> UpdateOne(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update,
            FindOneAndUpdateOptions<TDocument> options);

        Task<TDocument> UpdateOneById(string id, UpdateDefinition<TDocument> update,
            FindOneAndUpdateOptions<TDocument> options);

        Task<DeleteResult> RemoveOneById(string id);
        Task UpdateMany(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update);
    }

    public abstract class WriteDbOperationsService<TDocument> : IWriteDbOperationsService<TDocument> where TDocument : IDDocument 
    {
        private readonly IMongoCollection<TDocument> _collection;

        protected readonly FilterDefinitionBuilder<TDocument> Filter = Builders<TDocument>.Filter;
        protected readonly UpdateDefinitionBuilder<TDocument> Update = Builders<TDocument>.Update;

        protected WriteDbOperationsService(IMongoContext mongoContext, string collectionName)
        {
            _collection = mongoContext.Client.GetDatabase("reactivities").GetCollection<TDocument>(collectionName);
        }

        public async Task<TDocument> UpdateOne(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument> options = null)
        {
            if (options == null)
            {
                options = new FindOneAndUpdateOptions<TDocument>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After,
                };
            }
            
            return await _collection.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<TDocument> UpdateOneById(string id, UpdateDefinition<TDocument> update,
            FindOneAndUpdateOptions<TDocument> options = null)
        {
            return await UpdateOne(Filter.Eq(x => x.Id, id), update, options);
        }

        public async Task<DeleteResult> RemoveOneById(string id)
        {
           return await _collection.DeleteOneAsync(Filter.Eq(x => x.Id, id));
        }

        public async Task<TDocument> InsertOne(TDocument doc)
        {
            await _collection.InsertOneAsync(doc);
            return doc;
        }

        public async Task UpdateMany(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update)
        {
            await _collection.UpdateManyAsync(filter, update);
        }
    }
}