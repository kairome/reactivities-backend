using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Api
{
    public class DbMigrator : IStartupTask
    {
        private readonly IServiceProvider _serviceProvider;

        public DbMigrator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();

                try
                {
                    var db = mongoClient.GetDatabase("reactivities");
                    var collections = await db.ListCollections().ToListAsync(cancellationToken);
                    var usersCollectionExists = collections.Exists(x => x.GetValue("name").ToString() == "Users");
                    var activitiesCollectionExists = collections.Exists(x => x.GetValue("name").ToString() == "Activities");

                    if (!usersCollectionExists)
                    {
                        await db.CreateCollectionAsync("Users", cancellationToken: cancellationToken);
                    }
                    
                    if (!activitiesCollectionExists)
                    {
                        await db.CreateCollectionAsync("Activities", cancellationToken: cancellationToken);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to check existing collections", e);
                }
            }
        }
    }
}