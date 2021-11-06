using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Users;
using Domain;
using MongoDB.Driver;
using Persistence;

namespace Migrator.Migrate
{
    public class MigrateUsers : MigratorRunner<User, GetUsersService, WriteUsersService>, IMigratorRunner
    {

        public MigrateUsers(IMongoContext mongoContext) : base(mongoContext)
        {
        }
        
        public async Task Run()
        {
            Console.WriteLine("Running users migrations...");
        }
    }
}