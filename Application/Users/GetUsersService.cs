using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using Domain;
using MongoDB.Driver;
using Persistence;

namespace Application.Users
{
    [ScopedService]
    public interface IGetUsersService : IGetDbOperationsService<User>
    {
        Task<User> GetUserById(string userId);
        Task<List<User>> GetUsersByNameOrEmail(string userName, string email = null);
        Task<List<User>> GetUsersByIds(List<string> ids);
    }
    
    public class GetUsersService : GetDbOperationsService<User>, IGetUsersService
    {
        public GetUsersService(IMongoContext context) : base(context, "Users")
        {
        }

        public async Task<User> GetUserById(string userId)
        {
            return await GetOneById(userId);
        }

        public async Task<List<User>> GetUsersByNameOrEmail(string userName, string email = null)
        {
            var filters = new List<FilterDefinition<User>>
            {
                Filter.Eq(x => x.UserName, userName)
            };

            if (!string.IsNullOrEmpty(email))
            {
                filters.Add(Filter.Eq(x => x.Email, email));
            }

            return await GetAll(Filter.Or(filters));
        }

        public async Task<List<User>> GetUsersByIds(List<string> ids)
        {
            return await GetAll(Filter.In(x => x.Id, ids));
        }
    }
}