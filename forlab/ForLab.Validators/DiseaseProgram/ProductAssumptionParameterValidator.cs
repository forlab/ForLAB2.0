using FluentValidation;
using ForLab.DTO.DiseaseProgram.ProductAssumptionParameter;
using ForLab.Services.DiseaseProgram.ProductAssumptionParameter;

namespace ForLab.Validators.ProductAssumptionParameter
{
    public class ProductAssumptionParameterValidator : AbstractValidator<ProductAssumptionParameterDto>
    {
        readonly IProductAssumptionParameterService _productAssumptionParameterService;
        public ProductAssumptionParameterValidator(IProductAssumptionParameterService productAssumptionParameterService)
        {
            _productAssumptionParameterService = productAssumptionParameterService;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull()
                .Must(BeUniqueName).WithMessage("The name is already exist please try a new one");
            RuleFor(x => x.ProgramId)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
        }
        private bool BeUniqueName(ProductAssumptionParameterDto productAssumptionParameterDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _productAssumptionParameterService.IsNameUnique(productAssumptionParameterDto);
        }
    }

    public class ImportProductAssumptionParameterValidator : AbstractValidator<ProductAssumptionParameterDto>
    {
        public ImportProductAssumptionParameterValidator()
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
