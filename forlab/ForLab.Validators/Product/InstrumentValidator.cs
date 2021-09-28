using FluentValidation;
using ForLab.DTO.Product.Instrument;
using ForLab.Services.Product.Instrument;

namespace ForLab.Validators.Product
{
    public class InstrumentValidator : AbstractValidator<InstrumentDto>
    {
        readonly IInstrumentService _instrumentService;
        readonly int LoggedInUserId;
        readonly bool IsSuperAdmin;
        public InstrumentValidator(IInstrumentService instrumentService, int loggedInUserId, bool isSuperAdmin)
        {
            _instrumentService = instrumentService;
            LoggedInUserId = loggedInUserId;
            IsSuperAdmin = isSuperAdmin;

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueName).WithMessage("The Instrument name is already exist please try a new one");
            RuleFor(x => x.TestingAreaId)
                .NotEmpty()
                .GreaterThan(0)
                .NotNull();
            RuleFor(x => x.ReagentSystemId)
                .NotEmpty()
                .GreaterThan(0)
                .NotNull();
            RuleFor(x => x.ThroughPutUnitId)
               .NotEmpty()
               .GreaterThan(0)
               .NotNull();
            RuleFor(x => x.ControlRequirementUnitId)
              .NotEmpty()
              .NotNull();
            RuleFor(x => x.ThroughPutUnitId)
               .NotEmpty()
               .GreaterThan(0)
               .NotNull();
            RuleFor(x => x.MaxThroughPut)
               .NotEmpty()
               .GreaterThan(0)
               .NotNull();
        }
        private bool BeUniqueName(InstrumentDto instrumentDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _instrumentService.IsNameUnique(instrumentDto, LoggedInUserId, IsSuperAdmin);
        }
    }
    public class ImportInstrumentValidator : AbstractValidator<InstrumentDto>
    {
        public ImportInstrumentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.TestingAreaId)
                .NotEmpty()
                .GreaterThan(0)
                .NotNull();
            RuleFor(x => x.ReagentSystemId)
                .NotEmpty()
                .GreaterThan(0)
                .NotNull();
            RuleFor(x => x.ThroughPutUnitId)
               .NotEmpty()
               .GreaterThan(0)
               .NotNull();
            RuleFor(x => x.ControlRequirementUnitId)
              .NotEmpty()
              .GreaterThan(0)
              .NotNull();
            RuleFor(x => x.ThroughPutUnitId)
               .NotEmpty()
               .GreaterThan(0)
               .NotNull();
            RuleFor(x => x.MaxThroughPut)
               .NotEmpty()
               .GreaterThan(0)
               .NotNull();
        }
    }
}
