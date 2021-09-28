using FluentValidation;
using ForLab.DTO.Laboratory.LaboratoryPatientStatistic;
using ForLab.Services.Laboratory.LaboratoryPatientStatistic;
using System;

namespace ForLab.Validators.LaboratoryPatientStatistic
{
    public class LaboratoryPatientStatisticValidator : AbstractValidator<LaboratoryPatientStatisticDto>
    {
        readonly ILaboratoryPatientStatisticService _LaboratoryPatientStatisticService;
        public LaboratoryPatientStatisticValidator(ILaboratoryPatientStatisticService LaboratoryPatientStatisticService)
        {
            _LaboratoryPatientStatisticService = LaboratoryPatientStatisticService;

            RuleFor(x => x.LaboratoryId)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Count)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.Period)
               .NotEmpty()
               .NotNull()
               .Must(IsPatientStatisticUnique).WithMessage("This period is already exist with this laboratory");
        }

        private bool IsPatientStatisticUnique(LaboratoryPatientStatisticDto laboratoryPatientStatisticDto, DateTime newValue)
        {
            return _LaboratoryPatientStatisticService.IsPatientStatisticUnique(laboratoryPatientStatisticDto);
        }
    }
    public class ImportLaboratoryPatientStatisticValidator : AbstractValidator<LaboratoryPatientStatisticDto>
    {
        public ImportLaboratoryPatientStatisticValidator()
        {


            RuleFor(x => x.LaboratoryId)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Count)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.Period)
               .NotEmpty()
               .NotNull();
        }

    }
}
