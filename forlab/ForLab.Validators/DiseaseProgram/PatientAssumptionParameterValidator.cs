using FluentValidation;
using ForLab.DTO.DiseaseProgram.PatientAssumptionParameter;
using ForLab.Services.DiseaseProgram.PatientAssumptionParameter;

namespace ForLab.Validators.PatientAssumptionParameter
{
    public class PatientAssumptionParameterValidator : AbstractValidator<PatientAssumptionParameterDto>
    {
        readonly IPatientAssumptionParameterService _patientAssumptionParameterService;
        public PatientAssumptionParameterValidator(IPatientAssumptionParameterService patientAssumptionParameterService)
        {
            _patientAssumptionParameterService = patientAssumptionParameterService;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull()
                .Must(BeUniqueName).WithMessage("The name is already exist please try a new one");
            RuleFor(x => x.ProgramId)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
        }
        private bool BeUniqueName(PatientAssumptionParameterDto patientAssumptionParameterDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _patientAssumptionParameterService.IsNameUnique(patientAssumptionParameterDto);
        }
    }

    public class ImportPatientAssumptionParameterValidator : AbstractValidator<PatientAssumptionParameterDto>
    {
        public ImportPatientAssumptionParameterValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.ProgramId)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
        }
    }
}
