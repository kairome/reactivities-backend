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
            // await AddBioField();
        }

        private async Task AddPhotoField()
        {
            Console.WriteLine("Adding photo fields to users");
            await _writeService.UpdateMany(Filter.Empty, Update.Set(x => x.Photos, new List<Photo>()).Set(x => x.ProfilePhoto, null));
            Console.WriteLine("Photo fields added!");
        }

        private async Task AddBioField()
        {
            Console.WriteLine("Adding bio field to users");
            await _writeService.UpdateMany(Filter.Empty, Update.Set(x => x.Bio, null));

        }
    }
}