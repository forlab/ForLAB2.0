using FluentValidation;
using ForLab.Data.Enums;
using ForLab.DTO.Security.User;
using System.Text.RegularExpressions;

namespace ForLab.Validators.Security.User
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        private readonly Regex EmailRegex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        private readonly Regex PhoneRegex = new Regex(@"^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}");
        private readonly Regex FirstAndLastNameRegex = new Regex(@"^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z]+[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z-_]*$");

        public UserValidator()
        {
            RuleFor(x => x.UserSubscriptionLevelId)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.JobTitle)
                .NotEmpty().NotNull().WithMessage("Job Title cannot be empty")
                .Length(2, 50).WithMessage("Job Title length should not exceed 50 character");

            RuleFor(x => x.FirstName)
                .NotEmpty().NotNull().WithMessage("First Name cannot be empty")
                .Length(2, 50).WithMessage("First Name length should not exceed 50 character")
                .Matches(FirstAndLastNameRegex).WithMessage("First Name only allow characters");

            RuleFor(x => x.LastName)
                .NotEmpty().NotNull().WithMessage("Last Name cannot be empty")
                .Length(2, 50).WithMessage("First Name length should not exceed 50 character")
                .Matches(FirstAndLastNameRegex).WithMessage("Last Name only allow characters");

            RuleFor(x => x.Email)
                .NotEmpty().NotNull().WithMessage("Email cannot be empty")
                .Matches(EmailRegex).WithMessage("User email must be in a valid format. For Example: xyz@yahoo.com");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().NotNull().WithMessage("Phone cannot be empty")
                .Must(BeValidPhone).WithMessage("Please type a valid phone number");
                //.Matches(PhoneRegex).WithMessage("User phone number must be in a valid format. For Example: 123456789");

            RuleFor(x => x.Status)
                .NotEmpty().NotNull().WithMessage("Please select valid user status")
                .IsEnumName(typeof(UserStatusEnum), caseSensitive: true).WithMessage("Invalid user status value")
                .Must(BeNotLockedStatus).WithMessage("You can not lock the user account, only the system can");
        }

        private bool BeNotLockedStatus(UserDto userDto, string newValue)
        {
            if (newValue == UserStatusEnum.Locked.ToString())
            {
                return false;
            }
            return true;
        }
        private bool BeValidPhone(UserDto userDto, string newValue)
        {
            return (new GeneralValidators()).IsPhoneValid(newValue);
        }
    }
}
