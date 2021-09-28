using FluentValidation;
using ForLab.DTO.Disease.CountryDiseaseIncident;
using ForLab.Services.Disease.CountryDiseaseIncident;

namespace ForLab.Validators.CountryDiseaseIncident
{
    public class CountryDiseaseIncidentValidator : AbstractValidator<CountryDiseaseIncidentDto>
    {
        readonly ICountryDiseaseIncidentService _countryDiseaseIncidentService;
        public CountryDiseaseIncidentValidator(ICountryDiseaseIncidentService countryDiseaseIncidentService)
        {
            _countryDiseaseIncidentService = countryDiseaseIncidentService;
            RuleFor(x => x.DiseaseId)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueIncident).WithMessage("The country disease is already exist with this year");
            RuleFor(x => x.CountryId)
               .GreaterThan(0)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.Year)
              .GreaterThan(0)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.IncidencePer1kPopulation)
              .GreaterThan(0)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.IncidencePer100kPopulation)
              .GreaterThan(0)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.PrevalenceRate)
              .GreaterThan(0)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.PrevalenceRatePer1kPopulation)
             .GreaterThan(0)
             .NotEmpty()
             .NotNull();
           RuleFor(x => x.PrevalenceRatePer100kPopulation)
             .GreaterThan(0)
             .NotEmpty()
             .NotNull();

        }

        private bool BeUniqueIncident(CountryDiseaseIncidentDto countryDiseaseIncidentDto, int newValue)
        {
            return _countryDiseaseIncidentService.IsIncidentUnique(countryDiseaseIncidentDto);
        }
    }

    public class ImportCountryDiseaseIncidentValidator : AbstractValidator<CountryDiseaseIncidentDto>
    {
        public ImportCountryDiseaseIncidentValidator()
        {
            RuleFor(x => x.DiseaseId)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.CountryId)
               .GreaterThan(0)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.Year)
              .GreaterThan(0)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.IncidencePer1kPopulation)
              .GreaterThan(0)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.IncidencePer100kPopulation)
              .GreaterThan(0)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.PrevalenceRate)
              .GreaterThan(0)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.PrevalenceRatePer1kPopulation)
             .GreaterThan(0)
             .NotEmpty()
             .NotNull();
            RuleFor(x => x.PrevalenceRatePer100kPopulation)
              .GreaterThan(0)
              .NotEmpty()
              .NotNull();
        }
    }

}
