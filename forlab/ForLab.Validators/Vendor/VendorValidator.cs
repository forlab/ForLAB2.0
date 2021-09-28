using FluentValidation;
using ForLab.DTO.Vendor.Vendor;
using ForLab.Services.Vendor.Vendor;

namespace ForLab.Validators.Vendor
{
    public class VendorValidator : AbstractValidator<VendorDto>
    {
        readonly IVendorService _vendorService;
        readonly int LoggedInUserId;
        readonly bool IsSuperAdmin;
        public VendorValidator(IVendorService vendorService, int loggedInUserId, bool isSuperAdmin)
        {
            _vendorService = vendorService;
            LoggedInUserId = loggedInUserId;
            IsSuperAdmin = isSuperAdmin;

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueName).WithMessage("The vendor name is already exist please try a new one");
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress()
                .Must(BeUniqueEmail).WithMessage("The vendor email is already exist please try a new one");
            RuleFor(x => x.Telephone)
                .NotEmpty()
                .NotNull()
                .Must(BeValidPhone).WithMessage("Please type a valid phone number");
        }
        private bool BeUniqueName(VendorDto vendorDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _vendorService.IsNameUnique(vendorDto, LoggedInUserId, IsSuperAdmin);
        }
        private bool BeUniqueEmail(VendorDto vendorDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _vendorService.IsEmailUnique(vendorDto, LoggedInUserId, IsSuperAdmin);
        }
        private bool BeValidPhone(VendorDto vendorDto, string newValue)
        {
            return (new GeneralValidators()).IsPhoneValid(newValue);
        }
    }

    public class ImportVendorValidator : AbstractValidator<VendorDto>
    {
        public ImportVendorValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress();
            RuleFor(x => x.Telephone)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Address)
              .NotEmpty()
              .NotNull();
        }
    }

}
