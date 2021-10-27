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
    }

    public class GetActivitiesService : GetDbOperationsService<Activity>, IGetActivitiesService
    {
        public GetActivitiesService(IMongoContext context) : base(context, "Activities")
        {
        }

        public async Task<List<Activity>> GetActivities(ActivityFiltersDto filtersDto, string userId)
        {
            var sort = filtersDto.DateSort == ActivityDateSort.Asc
                ? Sort.Ascending(x => x.Date)
                : Sort.Descending(x => x.Date);

            var filters = new List<FilterDefinition<Activity>>
            {
                Filter.Empty,
            };

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
                filters.Add(Filter.Eq(x => x.AuthorId, userId)); 
            }

            return await GetAll(Filter.And(filters), sort);
        }

        public async Task<List<string>> GetCategories()
        {
            return await GetProjections(Projection.Expression(x => x.Category));
        }

        public async Task<List<string>> GetCities()
        {
            return await GetProjections(Projection.Expression(x => x.City));
        }
    }
}