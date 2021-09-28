using FluentValidation;
using ForLab.DTO.Vendor.VendorContact;
using ForLab.Services.Vendor.VendorContact;

namespace ForLab.Validators.Vendor
{
    public class VendorContactValidator : AbstractValidator<VendorContactDto>
    {
        readonly IVendorContactService _vendorContactService;
        public VendorContactValidator(IVendorContactService vendorContactService)
        {
            _vendorContactService = vendorContactService;

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueName).WithMessage("The contact name is already exist please try a new one");
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .EmailAddress()
                .Must(BeUniqueEmail).WithMessage("The contact email is already exist please try a new one");
            RuleFor(x => x.Telephone)
                .NotEmpty()
                .NotNull()
                .Must(BeValidPhone).WithMessage("Please type a valid phone number");
        }
        private bool BeUniqueName(VendorContactDto vendorContactDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _vendorContactService.IsNameUnique(vendorContactDto);
        }
        private bool BeUniqueEmail(VendorContactDto vendorContactDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _vendorContactService.IsEmailUnique(vendorContactDto);
        }
        private bool BeValidPhone(VendorContactDto vendorContactDto, string newValue)
        {
            return (new GeneralValidators()).IsPhoneValid(newValue);
        }
    }
    public class ImportVendorContactValidator : AbstractValidator<VendorContactDto>
    {
        public ImportVendorContactValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Telephone)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.VendorId)
            .NotEmpty()
            .GreaterThan(0)
            .NotNull();
        }
    }

}
