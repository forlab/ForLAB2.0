using FluentValidation;
using ForLab.DTO.Configuration;

namespace ForLab.Validators.Configuration
{
    public class ConfigurationValidator : AbstractValidator<ConfigurationDto> 
    {
        public ConfigurationValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.NumOfDaysToChangePassword)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.AccountLoginAttempts)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.UserPhotosize)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.PasswordExpiryTime)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.TimeToSessionTimeOut)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.AttachmentsMaxSize)
                 .NotEmpty()
                 .NotNull()
                 .GreaterThan(0);
        }
    }
}
