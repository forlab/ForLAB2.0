using FluentValidation;
using ForLab.DTO.Lookup.Country;
using ForLab.Services.Lookup.Country;

namespace ForLab.Validators.Lookup
{
    public class CountryValidator : AbstractValidator<CountryDto>
    {
        readonly ICountryService _countryService;
        public CountryValidator(ICountryService countryService)
        {
            _countryService = countryService;

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueName).WithMessage("The country name is already exist please try a new one");
            RuleFor(x => x.ShortCode)
              .NotEmpty()
              .NotNull()
              .Must(BeUniqueShortCode).WithMessage("The  Short Code is already exist please try a new one");
            RuleFor(x => x.ContinentId)
                .NotEmpty()
                .GreaterThan(0)
                .NotNull();
            RuleFor(x => x.CountryPeriodId)
                .NotEmpty()
                .GreaterThan(0)
                .NotNull();
            RuleFor(x => x.Latitude)
               .NotEmpty()
               .NotNull()
               .Must(BeUniqueLatlng).WithMessage("The country Latitude & Longitude are already exist please try a new one");
            RuleFor(x => x.Longitude)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.ShortName)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.NativeName)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.CurrencyCode)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.CallingCode)
             .NotEmpty()
             .NotNull();
            RuleFor(x => x.Population)
             .NotEmpty()
             .GreaterThan(0)
             .NotNull();
        }
        private bool BeUniqueName(CountryDto countryDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _countryService.IsNameUnique(countryDto);
        }
        private bool BeUniqueShortCode(CountryDto countryDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _countryService.IsShortCodeUnique(countryDto);
        }
        private bool BeUniqueLatlng(CountryDto countryDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _countryService.IsLatlngUnique(countryDto);
        }
    }


    public class ImportCountryValidator : AbstractValidator<CountryDto>
    {
        public ImportCountryValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.ShortCode)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.ContinentId)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.CountryPeriodId)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Latitude)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.Longitude)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.ShortName)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.NativeName)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.CurrencyCode)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.CallingCode)
             .NotEmpty()
             .NotNull();
            RuleFor(x => x.Population)
             .NotEmpty()
             .NotNull();
        }
    }
}
