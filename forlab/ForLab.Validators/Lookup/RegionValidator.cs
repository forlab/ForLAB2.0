using FluentValidation;
using ForLab.DTO.Lookup.Region;
using ForLab.Services.Lookup.Region;

namespace ForLab.Validators.Lookup
{
    public class RegionValidator : AbstractValidator<RegionDto>
    {
        readonly IRegionService _regionService;
        public RegionValidator(IRegionService regionService)
        {
            _regionService = regionService;

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueName).WithMessage("The Region name is already exist please try a new one");
            RuleFor(x => x.CountryId)
                .NotEmpty()
                .NotNull();
        }
        private bool BeUniqueName(RegionDto regionDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _regionService.IsNameUnique(regionDto);
        }
    }



    public class ImportRegionValidator : AbstractValidator<RegionDto>
    {
        public ImportRegionValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.CountryId)
              .NotEmpty()
              .NotNull();
        }
    }
}
