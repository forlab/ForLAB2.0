using FluentValidation;
using ForLab.DTO.Product.Product;
using ForLab.Services.Product.Product;

namespace ForLab.Validators.Product
{
    public class ProductValidator : AbstractValidator<ProductDto>
    {
        readonly IProductService _productService;
        readonly int LoggedInUserId;
        readonly bool IsSuperAdmin;
        public ProductValidator(IProductService productService, int loggedInUserId, bool isSuperAdmin)
        {
            _productService = productService;
            LoggedInUserId = loggedInUserId;
            IsSuperAdmin = isSuperAdmin;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull()
                .Must(BeUniqueName).WithMessage("The product name is already exist please try a new one");
            RuleFor(x => x.ProductBasicUnitId)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.VendorId)
               .GreaterThan(0)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.ProductTypeId)
               .GreaterThan(0)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.CatalogNo)
               .NotEmpty()
               .NotNull()
               .Must(IsCatalogNoUnique).WithMessage("The product catalog number are already exist please try a new one");
            RuleFor(x => x.ManufacturerPrice)
               .GreaterThan(0)
               .NotEmpty()
               .NotNull();

        }
        private bool BeUniqueName(ProductDto productDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _productService.IsNameUnique(productDto, LoggedInUserId, IsSuperAdmin);
        }
        private bool IsCatalogNoUnique(ProductDto productDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _productService.IsCatalogNoUnique(productDto);
        }
    }
    public class ImportProductValidator: AbstractValidator<ProductDto>
    {
        public ImportProductValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.ProductBasicUnitId)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.VendorId)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.ProductTypeId)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.CatalogNo)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.ManufacturerPrice)
               .NotEmpty()
               .NotNull();
        }
    }
}

