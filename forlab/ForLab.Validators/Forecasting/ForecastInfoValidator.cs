using FluentValidation;
using ForLab.Data.Enums;
using ForLab.DTO.Forecasting.ForecastInfo;
using ForLab.DTO.Forecasting.ForecastInstrument;
using ForLab.DTO.Forecasting.ForecastPatientGroup;
using ForLab.Services.Forecasting.ForecastInfo;
using System.Collections.Generic;
using System.Linq;

namespace ForLab.Validators.Forecasting
{
    public class ForecastInfoValidator : AbstractValidator<ForecastInfoDto>
    {
        readonly IForecastInfoService _forecastInfoService;
        readonly int LoggedInUserId;
        readonly bool IsSuperAdmin;
        public ForecastInfoValidator(IForecastInfoService forecastInfoService, int loggedInUserId, bool isSuperAdmin)
        {
            _forecastInfoService = forecastInfoService;
            LoggedInUserId = loggedInUserId;
            IsSuperAdmin = isSuperAdmin;

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueName).WithMessage("The name is already exist please try a new one");
            RuleFor(x => x.ForecastInfoLevelId)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.CountryId)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.ForecastMethodologyId)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.ScopeOfTheForecastId)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Duration)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.EndDate)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.WastageRate)
               .NotNull()
               .GreaterThanOrEqualTo(0)
               .LessThanOrEqualTo(100);
            RuleFor(x => x.ForecastInstrumentDtos)
                .Must(BeUniqueForecastInstrument).WithMessage("You should not duplicate the 'ForecastInfo Instrument'");
            RuleFor(x => x.ForecastPatientGroupDtos)
                .Must(BeValidPatientGroup).WithMessage("Sum of 'Patient Groups Percentage' must be 100% per perogram");
        }
        private bool BeUniqueName(ForecastInfoDto forecastInfoDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _forecastInfoService.IsNameUnique(forecastInfoDto, LoggedInUserId, IsSuperAdmin);
        }
        private bool BeUniqueForecastInstrument(ForecastInfoDto forecastInfoDto, List<ForecastInstrumentDto> newValue)
        {
            var duplicates = forecastInfoDto.ForecastInstrumentDtos.Where(x => !x.IsDeleted).GroupBy(x => x.InstrumentId).Any(g => g.Count() > 1);
            if (duplicates)
            {
                return false;
            }
            return true;
        }
        private bool BeValidPatientGroup(ForecastInfoDto forecastInfoDto, List<ForecastPatientGroupDto> newValue)
        {
            if(forecastInfoDto.ForecastMethodologyId != (int)ForecastMethodologyEnum.DempgraphicMorbidity)
            {
                return true;
            }
            var notValid = forecastInfoDto.ForecastPatientGroupDtos
                                    .Where(x => !x.IsDeleted)
                                    .GroupBy(x => new
                                    {
                                        ProgramId = x.ProgramId
                                    })
                                    .Any(g => g.Sum(x => x.Percentage) != 100);
            if (notValid)
            {
                return false;
            }
            return true;
        }
    }
}
