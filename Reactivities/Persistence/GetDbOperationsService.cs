using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using MongoDB.Driver;

namespace Persistence
{
    public interface IGetDbOperationsService<TDocument> where TDocument : IDDocument
    {
        Task<List<TDocument>> GetAll(FilterDefinition<TDocument> filters, SortDefinition<TDocument> sort);
        Task<List<TProjection>> GetProjections<TProjection>(ProjectionDefinition<TDocument, TProjection> projection, FilterDefinition<TDocument> filters);
        Task<TDocument> GetOneById(string id);
        Task<long> CountDocuments(FilterDefinition<TDocument> filters);
        Task<List<TDocument>> GetPaginatedList(FilterDefinition<TDocument> filters, SortDefinition<TDocument> sort, int currentPage, int pageSize);
    }

    public abstract class GetDbOperationsService<TDocument> : IGetDbOperationsService<TDocument>
        where TDocument : IDDocument
    {
        private readonly IMongoCollection<TDocument> _collection;

        protected readonly FilterDefinitionBuilder<TDocument> Filter = Builders<TDocument>.Filter;
        protected readonly SortDefinitionBuilder<TDocument> Sort = Builders<TDocument>.Sort;
        protected readonly ProjectionDefinitionBuilder<TDocument> Projection = Builders<TDocument>.Projection;

        protected GetDbOperationsService(IMongoContext mongoContext, string collectionName)
        {
            _collection = mongoContext.Client.GetDatabase("reactivities").GetCollection<TDocument>(collectionName);
        }

        public async Task<List<TDocument>> GetAll(FilterDefinition<TDocument> filters = null,
            SortDefinition<TDocument> sort = null)
        {
            var find = _collection.Find(filters ?? Filter.Empty).Sort(sort);

            return await find.ToListAsync();
        }

        public async Task<List<TProjection>> GetProjections<TProjection>(ProjectionDefinition<TDocument, TProjection> projection,
            FilterDefinition<TDocument> filters = null)
        {
            return await _collection.Find(filters ?? Filter.Empty).Project(projection).ToListAsync();
        }

        public async Task<TDocument> GetOneById(string id)
        {
            return await _collection.Find(Filter.Eq(x => x.Id, id)).FirstOrDefaultAsync();
        }

        public async Task<long> CountDocuments(FilterDefinition<TDocument> filters)
        {
            return await _collection.CountDocumentsAsync(filters);
        }

        public async Task<List<TDocument>> GetPaginatedList(FilterDefinition<TDocument> filters, SortDefinition<TDocument> sort, int currentPage, int pageSize)
        {
            var find = _collection.Find(filters ?? Filter.Empty).Sort(sort);
            find.Limit(pageSize).Skip(pageSize * (currentPage - 1));
            return await find.ToListAsync();
        }
    }
}