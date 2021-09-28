using FluentValidation;
using ForLab.DTO.Laboratory.LaboratoryConsumption;
using ForLab.Services.Laboratory.LaboratoryConsumption;
using System;

namespace ForLab.Validators.LaboratoryConsumption
{
    public class LaboratoryConsumptionValidator : AbstractValidator<LaboratoryConsumptionDto>
    {
        readonly ILaboratoryConsumptionService _laboratoryConsumption;
        public LaboratoryConsumptionValidator(ILaboratoryConsumptionService laboratoryConsumption)
        {
            _laboratoryConsumption = laboratoryConsumption;
            RuleFor(x => x.LaboratoryId)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.ConsumptionDuration)
               .NotEmpty()
               .NotNull()
               .Must(IsConsumptionUnique).WithMessage("This laboratory product is already exist with this duration");
        }

        private bool IsConsumptionUnique(LaboratoryConsumptionDto laboratoryConsumptionDto, DateTime newValue)
        {
            return _laboratoryConsumption.IsConsumptionUnique(laboratoryConsumptionDto);
        }
    }
    public class ImportLaboratoryConsumptionValidator : AbstractValidator<LaboratoryConsumptionDto>
    {
        public ImportLaboratoryConsumptionValidator()
        {

            RuleFor(x => x.LaboratoryId)
                  .NotEmpty()
                  .NotNull();
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.ConsumptionDuration)
               .NotEmpty()
               .NotNull();
        }

    }
}
