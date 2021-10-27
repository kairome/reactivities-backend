using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("/api/activities")]
    public class ActivitiesController : BaseController
    {
        private readonly IGetActivitiesService _getActivitiesService;
        private readonly IWriteActivitiesService _writeActivitiesService;
        private readonly ICurrentUserIdentity _currentUserIdentity;

        public ActivitiesController(
            IGetActivitiesService getActivitiesService,
            IWriteActivitiesService writeActivitiesService,
            ICurrentUserIdentity currentUserIdentity
        )
        {
            _getActivitiesService = getActivitiesService;
            _writeActivitiesService = writeActivitiesService;
            _currentUserIdentity = currentUserIdentity;
        }

        [HttpGet]
        public async Task<List<Activity>> GetActivities([FromQuery] ActivityFiltersDto filters)
        {
            var currentUser = await _currentUserIdentity.GetCurrentUser();
            return await _getActivitiesService.GetActivities(filters, currentUser?.Id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(string id)
        {
            var result = await _getActivitiesService.GetOneById(id);

            return HandleResult(Result<Activity>.Success(result));
        }

        [HttpPost]
        public async Task<Activity> CreateActivity(CreateActivityDto dto)
        {
            var currentUser = await _currentUserIdentity.GetCurrentUser();
            return await _writeActivitiesService.CreateActivity(dto, currentUser);
        }

        [Authorize(Policy = "IsActivityAuthor")]
        [HttpPut("{id}")]
        public async Task<Activity> UpdateActivity(string id, UpdateActivityDto dto)
        {
            return await _writeActivitiesService.UpdateActivity(id, dto);
        }

        [HttpPut("{id}/attend")]
        public async Task<Activity> AttendActivity(string id)
        {
            var activity = await _getActivitiesService.GetOneById(id);

            if (activity == null)
            {
                throw new BadRequest("Activity does not exist");
            }

            var currentUser = await _currentUserIdentity.GetCurrentUser();
            var existingAttendee = activity.Attendees.FirstOrDefault(x => x.UserId == currentUser.Id);
            if (existingAttendee != null)
            {
                throw new BadRequest("You are already attending this activity!");
            }

            return await _writeActivitiesService.AddAttendee(id, currentUser);
        }

        [HttpPut("{id}/leave")]
        public async Task<Activity> LeaveActivity(string id)
        {
            var activity = await _getActivitiesService.GetOneById(id);

            if (activity == null)
            {
                throw new BadRequest("Activity does not exist");
            }

            var currentUser = await _currentUserIdentity.GetCurrentUser();
            var existingAttendee = activity.Attendees.FirstOrDefault(x => x.UserId == currentUser.Id);
            if (existingAttendee == null)
            {
                throw new BadRequest("You are not attending this activity!");
            }

            return await _writeActivitiesService.RemoveAttendee(id, currentUser.Id);
        }

        [Authorize(Policy = "IsActivityAuthor")]
        [HttpPut("{id}/activate")]
        public async Task<Activity> ActivateActivity(string id)
        {
            var activity = await _getActivitiesService.GetOneById(id);

            if (activity == null)
            {
                throw new BadRequest("Activity does not exist");
            }


            return await _writeActivitiesService.UpdateActivityActiveStatus(id, false);
        }

        [Authorize(Policy = "IsActivityAuthor")]
        [HttpPut("{id}/cancel")]
        public async Task<Activity> CancelActivity(string id)
        {
            var activity = await _getActivitiesService.GetOneById(id);

            if (activity == null)
            {
                throw new BadRequest("Activity does not exist");
            }

            return await _writeActivitiesService.UpdateActivityActiveStatus(id, true);
        }

        [Authorize(Policy = "IsActivityAuthor")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActivity(string id)
        {
            var res = await _writeActivitiesService.DeleteActivity(id);
            if (res.IsSuccess)
            {
                return Ok();
            }

            return HandleResult(res);
        }

        [HttpGet("categories")]
        public async Task<List<string>> GetCategories()
        {
            return await _getActivitiesService.GetCategories();
        }

        [HttpGet("cities")]
        public async Task<List<string>> GetCities()
        {
            return await _getActivitiesService.GetCities();
        }
    }
}