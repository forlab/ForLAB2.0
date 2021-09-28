using FluentValidation;
using ForLab.DTO.Lookup.LaboratoryCategory;
using ForLab.Services.Lookup.LaboratoryCategory;

namespace ForLab.Validators.Lookup
{
    public class LaboratoryCategoryValidator : AbstractValidator<LaboratoryCategoryDto>
    {
        readonly ILaboratoryCategoryService _laboratoryCategoryService;
        readonly int LoggedInUserId;
        readonly bool IsSuperAdmin;
        public LaboratoryCategoryValidator(ILaboratoryCategoryService laboratoryCategoryService, int loggedInUserId, bool isSuperAdmin)
        {
            _laboratoryCategoryService = laboratoryCategoryService;
            LoggedInUserId = loggedInUserId;
            IsSuperAdmin = isSuperAdmin;

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueName).WithMessage("The laboratory Level name is already exist please try a new one");
        }
        private bool BeUniqueName(LaboratoryCategoryDto laboratoryCategoryDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _laboratoryCategoryService.IsNameUnique(laboratoryCategoryDto, LoggedInUserId, IsSuperAdmin);
        }
    }


    public class ImportLaboratoryCategoryValidator : AbstractValidator<LaboratoryCategoryDto>
    {
        public ImportLaboratoryCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull();
        }
    }
}
