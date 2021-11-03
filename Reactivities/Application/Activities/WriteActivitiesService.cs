using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Core;
using AspNetCore.ServiceRegistration.Dynamic;
using Domain;
using MongoDB.Driver;
using Persistence;

namespace Application.Activities
{
    [ScopedService]
    public interface IWriteActivitiesService : IWriteDbOperationsService<Activity>
    {
        Task<Activity> UpdateActivity(string id, UpdateActivityDto dto);
        Task<Result<object>> DeleteActivity(string id);
        Task<Activity> CreateActivity(CreateActivityDto createDto, User currentUser);
        Task<Activity> AddAttendee(string activityId, User user);
        Task<Activity> RemoveAttendee(string activityId, string userId);
        Task<Activity> UpdateActivityActiveStatus(string activityId, bool cancelled);
        Task AddComment(string activityId, Comment comment);
        Task<Activity> AddFollower(string activityId, ActivityFollower follower);
        Task<Activity> RemoveFollower(string activityId, string userId);
    }

    public class WriteActivitiesService : WriteDbOperationsService<Activity>, IWriteActivitiesService
    {
        public WriteActivitiesService(IMongoContext mongoContext) : base(
            mongoContext, "Activities")
        {
        }

        public async Task<Activity> UpdateActivity(string id, UpdateActivityDto dto)
        {
            var updates = new List<UpdateDefinition<Activity>>();

            if (!string.IsNullOrEmpty(dto.Title))
            {
                updates.Add(Update.Set(x => x.Title, dto.Title));
            }

            if (!string.IsNullOrEmpty(dto.Description))
            {
                updates.Add(Update.Set(x => x.Description, dto.Description));
            }

            if (!string.IsNullOrEmpty(dto.Category))
            {
                updates.Add(Update.Set(x => x.Category, dto.Category));
            }


            return await UpdateOne(Filter.Eq(x => x.Id, id), Update.Combine(updates));
        }

        public async Task<Result<object>> DeleteActivity(string id)
        {
            var result = await RemoveOneById(id);
            if (result.DeletedCount > 0)
            {
                return new Result<object> { IsSuccess = true, Value = null };
            }

            return new Result<object> { IsSuccess = false, Error = "Failed to delete the activity" };
        }

        public async Task<Activity> CreateActivity(CreateActivityDto dto, User currentUser)
        {

            var newActivity = await InsertOne(new Activity
            {
                Id = IdGenerator.GetNewId(),
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                Date = dto.Date,
                City = dto.City,
                Venue = dto.Venue,
                AuthorId = currentUser.Id,
                AuthorName = currentUser.DisplayName,
            });

            return newActivity;
        }

        public async Task<Activity> AddAttendee(string activityId, User user)
        {
            var newAttendee = new ActivityAttendee
            {
                UserId = user.Id,
                Name = user.DisplayName,
            };
            return await UpdateOne(Filter.Eq(x => x.Id, activityId), Update.Push(x => x.Attendees, newAttendee));
        }

        public async Task<Activity> RemoveAttendee(string activityId, string userId)
        {
            return await UpdateOne(Filter.Eq(x => x.Id, activityId),
                Update.PullFilter(x => x.Attendees, a => a.UserId == userId));
        }

        public async Task<Activity> UpdateActivityActiveStatus(string activityId, bool cancelled)
        {
            return await UpdateOne(Filter.Eq(x => x.Id, activityId), Update.Set(x => x.IsCancelled, cancelled));
        }

        public async Task AddComment(string activityId, Comment comment)
        {
            await UpdateOneById(activityId, Update.Push(x => x.Comments, comment));
        }

        public async Task<Activity> AddFollower(string activityId, ActivityFollower follower)
        {
            return await UpdateOneById(activityId, Update.Push(x => x.Followers, follower));
        }

        public async Task<Activity> RemoveFollower(string activityId, string userId)
        {
            return await UpdateOneById(activityId, Update.PullFilter(x => x.Followers, f => f.UserId == userId));
        }
    }
}