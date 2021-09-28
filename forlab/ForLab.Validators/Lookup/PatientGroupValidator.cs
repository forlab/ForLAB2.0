
using FluentValidation;
using ForLab.DTO.Lookup.PatientGroup;
using ForLab.Services.Lookup.PatientGroup;

namespace ForLab.Validators.Lookup
{
    public class PatientGroupValidator : AbstractValidator<PatientGroupDto>
    {
        readonly IPatientGroupService _patientGroupService;
        readonly int LoggedInUserId;
        readonly bool IsSuperAdmin;

        public PatientGroupValidator(IPatientGroupService patientGroupService, int loggedInUserId, bool isSuperAdmin)
        {
            _patientGroupService = patientGroupService;
            LoggedInUserId = loggedInUserId;
            IsSuperAdmin = isSuperAdmin;

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueName).WithMessage("The Patient Group name is already exist please try a new one");

            
        }
        private bool BeUniqueName(PatientGroupDto patientGroupDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _patientGroupService.IsNameUnique(patientGroupDto, LoggedInUserId, IsSuperAdmin);
        }
    }


    public class ImportPatientGroupValidator : AbstractValidator<PatientGroupDto>
    {
        public ImportPatientGroupValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull();

        }
    }
}
