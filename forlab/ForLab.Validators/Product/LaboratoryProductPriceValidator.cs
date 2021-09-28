using FluentValidation;
using ForLab.DTO.Product.LaboratoryProductPrice;
using ForLab.Services.Product.LaboratoryProductPrice;

namespace ForLab.Validators.Product
{
    public class LaboratoryProductPriceValidator : AbstractValidator<LaboratoryProductPriceDto>
    {
        readonly ILaboratoryProductPriceService _laboratoryProductPriceService;
        public LaboratoryProductPriceValidator(ILaboratoryProductPriceService laboratoryProductPriceService)
        {
            _laboratoryProductPriceService = laboratoryProductPriceService;

            RuleFor(x => x.LaboratoryId)
                  .GreaterThan(0)
                  .NotEmpty()
                  .NotNull()
                  .Must(IsLaboratoryProductUnique).WithMessage("This product is figured with a price for this laboratory.");
            RuleFor(x => x.ProductId)
                 .GreaterThan(0)
                 .NotEmpty()
                 .NotNull();
            RuleFor(x => x.Price)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.PackSize)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.FromDate)
                .NotEmpty()
                .NotNull();
        }

        private bool IsLaboratoryProductUnique(LaboratoryProductPriceDto laboratoryProductPriceDto, int newValue)
        {
            return _laboratoryProductPriceService.IsLaboratoryProductUnique(laboratoryProductPriceDto);
        }
    }
    public class ImportLaboratoryProductPriceValidator : AbstractValidator<LaboratoryProductPriceDto>
    {
        public ImportLaboratoryProductPriceValidator()
        {
            RuleFor(x => x.LaboratoryId)
                 .NotEmpty()
                 .GreaterThan(0)
                 .NotNull();
            RuleFor(x => x.ProductId)
                 .NotEmpty()
                 .GreaterThan(0)
                 .NotNull();
            RuleFor(x => x.Price)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.PackSize)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.FromDate)
               .NotEmpty()
               .NotNull();
        }
    }
}
