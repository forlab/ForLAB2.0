using FluentValidation;
using ForLab.DTO.CMS.UsefulResource;
using ForLab.Services.CMS.UsefulResource;
using System.Text.RegularExpressions;

namespace ForLab.Validators.CMS
{
    public class UsefulResourceValidator : AbstractValidator<UsefulResourceDto>
    {
        readonly IUsefulResourceService _usefulResourceService;
        public UsefulResourceValidator(IUsefulResourceService usefulResourceService)
        {
            _usefulResourceService = usefulResourceService;

            RuleFor(x => x.Title)
                   .NotEmpty()
                   .NotNull()
                   .Must(BeUniqueTitle).WithMessage("The Title is already exist please try a new one");
            RuleFor(x => x.AttachmentUrl)
                   .Must(BeValidUrl).WithMessage("Type a valid URL")
                   .Must(BeUniqueUrl).WithMessage("The URL is already exist please try a new one");
            RuleFor(x => x.AttachmentName)
                   .Must(BeUniqueAttachmentName).WithMessage("The Attachment Name is already exist please try a new one");

        }

        private bool BeUniqueTitle(UsefulResourceDto usefulResourceDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _usefulResourceService.IsTitleUnique(usefulResourceDto);
        }
        private bool BeUniqueAttachmentName(UsefulResourceDto usefulResourceDto, string newValue)
        {
            if (usefulResourceDto.IsExternalResource)
            {
                return true;
            }
            return _usefulResourceService.IsAttachmentNameUnique(usefulResourceDto);
        }
        private bool BeUniqueUrl(UsefulResourceDto usefulResourceDto, string newValue)
        {
            if (!usefulResourceDto.IsExternalResource)
            {
                return true;
            }
            return _usefulResourceService.IsUrlUnique(usefulResourceDto);
        }
        private bool BeValidUrl(UsefulResourceDto usefulResourceDto, string newValue)
        {
            if(!usefulResourceDto.IsExternalResource)
            {
                return true;
            }

            return (new GeneralValidators()).IsValidUrl(newValue);
        }
    }
}
