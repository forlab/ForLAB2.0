using FluentValidation;
using ForLab.DTO.Lookup.LaboratoryLevel;
using ForLab.Services.Lookup.LaboratoryLevel;

namespace ForLab.Validators.Lookup
{
    public class LaboratoryLevelValidator : AbstractValidator<LaboratoryLevelDto>
    {
        readonly ILaboratoryLevelService _laboratoryLevelService;
        readonly int LoggedInUserId;
        readonly bool IsSuperAdmin;
        public LaboratoryLevelValidator(ILaboratoryLevelService laboratoryLevelService, int loggedInUserId, bool isSuperAdmin)
        {
            _laboratoryLevelService = laboratoryLevelService;
            LoggedInUserId = loggedInUserId;
            IsSuperAdmin = isSuperAdmin;

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueName).WithMessage("The laboratory Level name is already exist please try a new one");
            
        }
        private bool BeUniqueName(LaboratoryLevelDto laboratoryLevelDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _laboratoryLevelService.IsNameUnique(laboratoryLevelDto, LoggedInUserId, IsSuperAdmin);
        }

    }
    public class ImportLaboratoryLevelValidator : AbstractValidator<LaboratoryLevelDto>
    {
        public ImportLaboratoryLevelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull();
        }
    }
}
