using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Migrator.Migrate;
using MongoDB.Driver;
using Persistence;

namespace Migrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();

            Console.WriteLine("Starting migrations, trying to establish the connection to mongodb...");
            var mongoClient =
                new MongoClient(
                    config.GetConnectionString("MongoUri"));
            var mongoContext = new MongoContext(mongoClient);
            Console.WriteLine("Mongodb connection established, context created!");

            var activitiesMigrations = new MigrateActivities(mongoContext);
            var usersMigrations = new MigrateUsers(mongoContext);

            var migrations = new List<IMigratorRunner>
            {
                activitiesMigrations,
                usersMigrations,
            };

            await RunMigrations(migrations);
        }

        private static async Task RunMigrations(List<IMigratorRunner> migrations)
        {
            foreach (var migration in migrations)
            {
                await migration.Run();
            }
        }
    }
}