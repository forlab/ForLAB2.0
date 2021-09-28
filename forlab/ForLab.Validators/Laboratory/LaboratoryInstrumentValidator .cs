using FluentValidation;
using ForLab.DTO.Laboratory.LaboratoryInstrument;
using ForLab.Services.Laboratory.LaboratoryInstrument;

namespace ForLab.Validators.LaboratoryInstrument
{
    public class LaboratoryInstrumentValidator : AbstractValidator<LaboratoryInstrumentDto>
    {
        readonly ILaboratoryInstrumentService _laboratoryInstrumentService;
        public LaboratoryInstrumentValidator(ILaboratoryInstrumentService laboratoryInstrumentService)
        {
            _laboratoryInstrumentService = laboratoryInstrumentService;

            RuleFor(x => x.LaboratoryId)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.InstrumentId)
                .NotEmpty()
                .NotNull()
                .Must(IsLaboratoryInstrumentUnique).WithMessage("This instrument is already assigned to this laboratory");
            RuleFor(x => x.Quantity)
               .NotEmpty()
               .NotNull()
               .GreaterThan(0);
            RuleFor(x => x.TestRunPercentage)
              .NotEmpty()
              .NotNull()
              .GreaterThan(0)
              .LessThanOrEqualTo(100)
              .Must(IsValidPercentage).WithMessage("Sum of all instrument test percentage must be 100% per testing area");
        }

        private bool IsLaboratoryInstrumentUnique(LaboratoryInstrumentDto laboratoryInstrumentDto, int newValue)
        {
            return _laboratoryInstrumentService.IsLaboratoryInstrumentUnique(laboratoryInstrumentDto);
        }
        private bool IsValidPercentage(LaboratoryInstrumentDto laboratoryInstrumentDto, decimal newValue)
        {
            return _laboratoryInstrumentService.IsValidPercentage(laboratoryInstrumentDto);
        }
    }
    public class ImportLaboratoryInstrumentValidator : AbstractValidator<LaboratoryInstrumentDto>
    {
        public ImportLaboratoryInstrumentValidator()
        {
            RuleFor(x => x.LaboratoryId)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.InstrumentId)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Quantity)
               .NotEmpty()
               .NotNull()
               .GreaterThan(0);
            RuleFor(x => x.TestRunPercentage)
              .NotEmpty()
              .NotNull()
              .GreaterThan(0)
              .LessThanOrEqualTo(100);
        }
    }
}
