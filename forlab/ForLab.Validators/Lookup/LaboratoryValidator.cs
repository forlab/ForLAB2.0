using FluentValidation;
using ForLab.DTO.Lookup.Laboratory;
using ForLab.Services.Lookup.Laboratory;

namespace ForLab.Validators.Lookup
{
    public class LaboratoryValidator : AbstractValidator<LaboratoryDto>
    {
        readonly ILaboratoryService _laboratoryService;
        readonly int LoggedInUserId;
        readonly bool IsSuperAdmin;
        public LaboratoryValidator(ILaboratoryService laboratoryService, int loggedInUserId, bool isSuperAdmin)
        {
            _laboratoryService = laboratoryService;
            LoggedInUserId = loggedInUserId;
            IsSuperAdmin = isSuperAdmin;

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueName).WithMessage("The Laboratory name is already exist please try a new one");
            RuleFor(x => x.LaboratoryCategoryId)
                .NotEmpty()
                .GreaterThan(0)
                .NotNull();
            RuleFor(x => x.LaboratoryLevelId)
                .NotEmpty()
                .GreaterThan(0)
                .NotNull();
            RuleFor(x => x.Latitude)
               .NotEmpty()
               .NotNull();
               //.Must(BeUniqueLatlng).WithMessage("The Laboratory Latitude & Longitude are already exist please try a new one");
            RuleFor(x => x.Longitude)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.RegionId)
               .NotEmpty()
               .GreaterThan(0)
               .NotNull();

        }
        private bool BeUniqueName(LaboratoryDto laboratoryDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _laboratoryService.IsNameUnique(laboratoryDto, LoggedInUserId, IsSuperAdmin);
        }
        private bool BeUniqueLatlng(LaboratoryDto laboratoryDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _laboratoryService.IsLatlngUnique(laboratoryDto);
        }
    }



    public class ImportLaboratoryValidator : AbstractValidator<LaboratoryDto>
    {
        public ImportLaboratoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.LaboratoryCategoryId)
                .NotEmpty()
                .GreaterThan(0)
                .NotNull();
            RuleFor(x => x.LaboratoryLevelId)
                .NotEmpty()
                .GreaterThan(0)
                .NotNull();
            RuleFor(x => x.Latitude)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.Longitude)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.RegionId)
               .NotEmpty()
               .GreaterThan(0)
               .NotNull();
        }
    }



}
