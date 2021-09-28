using FluentValidation;
using ForLab.DTO.Disease.Disease;
using ForLab.Services.Disease.Disease;

namespace ForLab.Validators.Disease
{
    public class DiseaseValidator : AbstractValidator<DiseaseDto>
    {
        readonly IDiseaseService _diseaseService;
        public DiseaseValidator(IDiseaseService diseaseService)
        {
            _diseaseService = diseaseService;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull()
                .Must(BeUniqueName).WithMessage("The name is already exist please try a new one");
        }
        private bool BeUniqueName(DiseaseDto diseaseDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _diseaseService.IsNameUnique(diseaseDto);
        }
    }



    public class ImportDiseaseValidator : AbstractValidator<DiseaseDto>
    {
        public ImportDiseaseValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull();
        }
    }
}
