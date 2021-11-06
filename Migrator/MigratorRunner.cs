using System;
using System.Threading.Tasks;
using Domain;
using MongoDB.Driver;
using Persistence;

namespace Migrator
{
    public interface IMigratorRunner
    {
        public Task Run();
    }
    
    public abstract class MigratorRunner<TDocument, TGetService, TWriteService>
        where TDocument : IDDocument
        where TGetService : GetDbOperationsService<TDocument>
        where TWriteService : WriteDbOperationsService<TDocument>
    {
        protected TGetService _getService { get; }
        protected TWriteService _writeService { get; }

        protected readonly FilterDefinitionBuilder<TDocument> Filter = Builders<TDocument>.Filter;
        protected readonly UpdateDefinitionBuilder<TDocument> Update = Builders<TDocument>.Update;
        
        protected MigratorRunner(IMongoContext mongoContext)
        {
            _getService =
                (TGetService)Activator.CreateInstance(typeof(TGetService), new object[] { mongoContext });
            _writeService =
                (TWriteService)Activator.CreateInstance(typeof(TWriteService), new object[] { mongoContext });
        }
    }
}