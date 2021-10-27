using System;
using Domain;
using FluentValidation;

namespace Application.Activities
{
    public class CreateActivitiesValidator : AbstractValidator<CreateActivityDto>
    {
        public CreateActivitiesValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Date).NotEmpty().GreaterThanOrEqualTo(DateTime.Now);
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.Venue).NotEmpty();
            RuleFor(x => x.Category).NotEmpty();
        }
    }
    
    public class UpdateActivitiesValidator : AbstractValidator<UpdateActivityDto>
    {
        public UpdateActivitiesValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Category).NotEmpty();
        }
    }
}