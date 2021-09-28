using FluentValidation;
using ForLab.DTO.Product.CountryProductPrice;
using ForLab.Services.Product.CountryProductPrice;

namespace ForLab.Validators.Product
{
    public class CountryProductPriceValidator : AbstractValidator<CountryProductPriceDto>
    {
        readonly ICountryProductPriceService _countryProductPriceService;
        public CountryProductPriceValidator(ICountryProductPriceService countryProductPriceService)
        {
            _countryProductPriceService = countryProductPriceService;

            RuleFor(x => x.CountryId)
                 .NotEmpty()
                 .GreaterThan(0)
                 .NotNull()
                 .Must(IsCountryProductUnique).WithMessage("This product is figured with a price for this country.");
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

        private bool IsCountryProductUnique(CountryProductPriceDto countryProductPriceDto, int newValue)
        {
            return _countryProductPriceService.IsCountryProductUnique(countryProductPriceDto);
        }
    }
    public class ImportCountryProductPriceValidator : AbstractValidator<CountryProductPriceDto>
    {
        public ImportCountryProductPriceValidator()
        {
            RuleFor(x => x.CountryId)
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
