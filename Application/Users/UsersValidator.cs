using Domain;
using FluentValidation;

namespace Application.Users
{
    public class UserRegistrationValidator : AbstractValidator<NewUserDto>
    {
        public UserRegistrationValidator()
        {
            RuleFor(x => x.DisplayName).NotEmpty();
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password)
                .NotEmpty()
                .Length(8, 20).WithMessage("Password must be between 8 and 20 characters long")
                .Matches("[A-Z]").WithMessage("Password must contain uppercase letters")
                .Matches("[a-z]").WithMessage("Password must contain lowercase letters")
                .Matches("[0-9]").WithMessage("Password must contain digits")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least 1 special character");
        }
    }

    public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).EmailAddress().NotEmpty();
            RuleFor(x => x.Bio).MaximumLength(200);
        }
    }
}