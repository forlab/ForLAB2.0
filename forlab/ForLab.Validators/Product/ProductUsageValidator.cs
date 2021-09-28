using FluentValidation;
using ForLab.DTO.Product.ProductUsage;
using ForLab.Services.Product.ProductUsage;

namespace ForLab.Validators.Product
{
    public class ProductUsageValidator : AbstractValidator<ProductUsageDto>
    {
        readonly IProductUsageService _productUsageService;
        public ProductUsageValidator(IProductUsageService productUsageService)
        {
            _productUsageService = productUsageService;
            RuleFor(x => x.ProductId)
               .NotEmpty()
               .NotNull()
               .GreaterThan(0);
            RuleFor(x => x.Amount)
                .NotEmpty()
                .NotNull()
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.TestId)
                .Must(BeUniqueTestUsage).WithMessage("You should not duplicate the Product with the same Test and Instrument");
            RuleFor(x => x.PerPeriod)
                .Must(BeUniqueProductUsagePeriod).WithMessage("You should not duplicate the Product with the same Period");
            RuleFor(x => x.PerPeriodPerInstrument)
                .Must(BeUniqueProductUsageInstrument).WithMessage("You should not duplicate the Product with the same Instrument");
        }

        private bool BeUniqueTestUsage(ProductUsageDto productUsageDto, int? newValue)
        {
            if(productUsageDto.TestId != null)
            {
                return _productUsageService.IsTestUsageUnique(productUsageDto);
            }
            return true;
        }
        private bool BeUniqueProductUsagePeriod(ProductUsageDto productUsageDto, bool newValue)
        {
            if(productUsageDto.PerPeriod)
            {
                return _productUsageService.IsProductUsagePeriodUnique(productUsageDto);
            }
            return true;
        }
        private bool BeUniqueProductUsageInstrument(ProductUsageDto productUsageDto, bool newValue)
        {
            if (productUsageDto.PerPeriodPerInstrument)
            {
                return _productUsageService.IsProductUsageInstrumentUnique(productUsageDto);
            }
            return true;
        }
    }

    public class ImportProductUsageValidator : AbstractValidator<ProductUsageDto>
    {
        public ImportProductUsageValidator()
        {
            RuleFor(x => x.ProductId)
               .NotEmpty()
               .NotNull()
               .GreaterThan(0);
            RuleFor(x => x.Amount)
               .NotEmpty()
               .NotNull()
               .GreaterThanOrEqualTo(0);
        }
    }
}
