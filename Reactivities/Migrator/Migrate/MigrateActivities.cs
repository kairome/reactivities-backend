using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Application.Users;
using Domain;
using Persistence;

namespace Migrator.Migrate
{
    public class MigrateActivities : MigratorRunner<Activity, GetActivitiesService, WriteActivitiesService>,
        IMigratorRunner
    {
        private readonly GetUsersService _getUsersService;

        public MigrateActivities(
            IMongoContext mongoContext
        ) : base(mongoContext)
        {
            _getUsersService = new GetUsersService(mongoContext);
        }

        public async Task Run()
        {
            Console.WriteLine("Running activities migrations...");
            // await UpdateActivityAttendees();
        }

        private async Task UpdateActivityAttendees()
        {
            Console.WriteLine("Updating activity attendees");
            var allUsers = await _getUsersService.GetAll();
            var allActivities = await _getService.GetAll();

            foreach (var activity in allActivities)
            {
                var newAttendees = activity.Attendees.Select(x =>
                {
                    var attendee = allUsers.FirstOrDefault(u => u.Id == x.UserId);
                    return new ActivityAttendee
                    {
                        UserId = attendee?.Id,
                        Name = attendee?.DisplayName,
                        PhotoUrl = attendee?.ProfilePhoto?.Url,
                    };
                });

                await _writeService.UpdateOne(Filter.Eq(x => x.Id, activity.Id),
                    Update.Set(x => x.Attendees, newAttendees));
            }
            
            Console.WriteLine("Attendees updated!");
        }
    }
}