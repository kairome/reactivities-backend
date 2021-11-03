using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using Domain;
using ExtensionMethods;
using MongoDB.Driver;
using Persistence;

namespace Application.Activities
{
    [ScopedService]
    public interface IGetActivitiesService : IGetDbOperationsService<Activity>
    {
        Task<List<Activity>> GetActivities(ActivityFiltersDto filters, string userId);
        Task<List<string>> GetCategories();
        Task<List<string>> GetCities();
        Task<long> CountActivitiesByFilter(ActivityFiltersDto filtersDto, string userId);
    }

    public class GetActivitiesService : GetDbOperationsService<Activity>, IGetActivitiesService
    {
        public GetActivitiesService(IMongoContext context) : base(context, "Activities")
        {
        }

        public async Task<List<Activity>> GetActivities(ActivityFiltersDto filtersDto, string userId)
        {
            var sort = filtersDto.DateSort is ActivityDateSort.Asc
                ? Sort.Ascending(x => x.Date)
                : Sort.Descending(x => x.Date);

            return await GetAll(GetActivitiesFilters(filtersDto, userId), sort);
        }

        public async Task<List<string>> GetCategories()
        {
            return await GetProjections(Projection.Expression(x => x.Category));
        }

        public async Task<List<string>> GetCities()
        {
            return await GetProjections(Projection.Expression(x => x.City));
        }

        public async Task<long> CountActivitiesByFilter(ActivityFiltersDto filtersDto, string userId)
        {
            return await CountDocuments(GetActivitiesFilters(filtersDto, userId));
        }

        private FilterDefinition<Activity> GetActivitiesFilters(
            ActivityFiltersDto filtersDto, string userId)
        {
            var filters = new List<FilterDefinition<Activity>>();

            var myActivitiesFilters = new List<FilterDefinition<Activity>>();

            if (!string.IsNullOrEmpty(filtersDto.Title))
            {
                filters.Add(Filter.Where(x => x.Title.ToLower().Contains(filtersDto.Title.ToLower())));
            }

            if (filtersDto.DateFrom != null && filtersDto.DateTo != null)
            {
                filters.Add(Filter.Gte(x => x.Date, filtersDto.DateFrom.Value.GetStartOfDay()));
                filters.Add(Filter.Lte(x => x.Date, filtersDto.DateTo.Value.GetEndOfDay()));
            }

            if (filtersDto.DateFrom != null && filtersDto.DateTo == null)
            {
                filters.Add(Filter.Gte(x => x.Date, filtersDto.DateFrom.Value.GetStartOfDay()));
                filters.Add(Filter.Lte(x => x.Date, filtersDto.DateFrom.Value.GetEndOfDay()));
            }

            if (filtersDto.Categories.Any())
            {
                filters.Add(Filter.In(x => x.Category, filtersDto.Categories));
            }

            if (filtersDto.Cities.Any())
            {
                filters.Add(Filter.In(x => x.City, filtersDto.Cities));
            }

            if (filtersDto.IsMy.HasValue && filtersDto.IsMy.Value)
            {
                myActivitiesFilters.Add(Filter.Eq(x => x.AuthorId, userId));
            }

            if (filtersDto.Attending.HasValue && filtersDto.Attending.Value)
            {
                myActivitiesFilters.Add(Filter.ElemMatch(x => x.Attendees, a => a.UserId == userId));
            }

            if (filtersDto.Following.HasValue && filtersDto.Following.Value)
            {
                myActivitiesFilters.Add(Filter.ElemMatch(x => x.Followers, f => f.UserId == userId));
            }

            if (filters.Any() && myActivitiesFilters.Any())
            {
                return Filter.And(Filter.And(filters), Filter.Or(myActivitiesFilters));
            }
            
            if (filters.Any())
            {
                return Filter.And(filters);
            }

            return myActivitiesFilters.Any() ? Filter.Or(myActivitiesFilters) : Filter.Empty;
        }
    }
}