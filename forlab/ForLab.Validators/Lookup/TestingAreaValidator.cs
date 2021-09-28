using FluentValidation;
using ForLab.DTO.Lookup.TestingArea;
using ForLab.Services.Lookup.TestingArea;

namespace ForLab.Validators.Lookup
{
    public class TestingAreaValidator : AbstractValidator<TestingAreaDto>
    {
        readonly ITestingAreaService _testingAreaService;
        readonly int LoggedInUserId;
        readonly bool IsSuperAdmin;

        public TestingAreaValidator(ITestingAreaService testingAreaService, int loggedInUserId, bool isSuperAdmin)
        {
            _testingAreaService = testingAreaService;
            LoggedInUserId = loggedInUserId;
            IsSuperAdmin = isSuperAdmin;

            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .Must(BeUniqueName).WithMessage("The Area name is already exist please try a new one");
        }
        private bool BeUniqueName(TestingAreaDto testingAreaDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _testingAreaService.IsNameUnique(testingAreaDto, LoggedInUserId, IsSuperAdmin);
        }
    }
    public class ImportTestingAreaValidator : AbstractValidator<TestingAreaDto>
    {
        public ImportTestingAreaValidator()
        {
            RuleFor(x => x.Name)
                  .NotEmpty()
                  .NotNull();
        }
    }
}
