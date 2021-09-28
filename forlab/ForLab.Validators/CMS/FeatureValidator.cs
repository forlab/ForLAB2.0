using FluentValidation;
using ForLab.DTO.CMS.Feature;
using ForLab.Services.CMS.Feature;

namespace ForLab.Validators.CMS
{
    public class FeatureValidator : AbstractValidator<FeatureDto>
    {
        readonly IFeatureService _featureService;
        public FeatureValidator(IFeatureService featureService)
        {
            _featureService = featureService;

            RuleFor(x => x.Title)
                   .NotEmpty()
                   .NotNull()
                   .Must(BeUniqueTitle).WithMessage("The Title is already exist please try a new one");
        }

        private bool BeUniqueTitle(FeatureDto featureDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _featureService.IsTitleUnique(featureDto);
        }
    }
}
