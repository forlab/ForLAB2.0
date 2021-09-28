using FluentValidation;
using ForLab.DTO.Product.RegionProductPrice;
using ForLab.Services.Product.RegionProductPrice;

namespace ForLab.Validators.Product
{
    public class RegionProductPriceValidator : AbstractValidator<RegionProductPriceDto>
    {
        readonly IRegionProductPriceService _regionProductPriceService;
        public RegionProductPriceValidator(IRegionProductPriceService regionProductPriceService)
        {
            _regionProductPriceService = regionProductPriceService;

            RuleFor(x => x.RegionId)
                .GreaterThan(0)
                 .NotEmpty()
                 .NotNull()
                 .Must(IsRegionProductUnique).WithMessage("This product is figured with a price for this region.");
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

        private bool IsRegionProductUnique(RegionProductPriceDto regionProductPriceDto, int newValue)
        {
            return _regionProductPriceService.IsRegionProductUnique(regionProductPriceDto);
        }
    }


    public class ImportRegionProductPriceValidator : AbstractValidator<RegionProductPriceDto>
    {
        public ImportRegionProductPriceValidator()
        {
            RuleFor(x => x.RegionId)
               .GreaterThan(0)
                .NotEmpty()
                .NotNull();
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
    }
}
