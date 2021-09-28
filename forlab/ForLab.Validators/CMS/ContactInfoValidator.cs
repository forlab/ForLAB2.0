using FluentValidation;
using ForLab.DTO.CMS.ContactInfo;

namespace ForLab.Validators.CMS
{
    public class ContactInfoValidator : AbstractValidator<ContactInfoDto>
    {
        public ContactInfoValidator()
        {
            RuleFor(x => x.Phone)
                 .NotEmpty()
                 .NotNull();
            RuleFor(x => x.Email)
                 .NotEmpty()
                 .NotNull();
            RuleFor(x => x.Address)
                 .NotEmpty()
                 .NotNull();
            RuleFor(x => x.Latitude)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Longitude)
                .NotEmpty()
                .NotNull();
        }

    }
}
