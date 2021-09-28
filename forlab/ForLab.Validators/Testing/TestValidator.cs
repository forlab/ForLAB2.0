using FluentValidation;
using ForLab.DTO.Testing.Test;
using ForLab.Services.Testing.Test;

namespace ForLab.Validators.Test
{
    public class TestValidator : AbstractValidator<TestDto>
    {
        readonly ITestService _testService;
        readonly int LoggedInUserId;
        readonly bool IsSuperAdmin;
        public TestValidator(ITestService testService, int loggedInUserId, bool isSuperAdmin)
        {
            _testService = testService;
            LoggedInUserId = loggedInUserId;
            IsSuperAdmin = isSuperAdmin;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull()
                .Must(BeUniqueName).WithMessage("The name is already exist please try a new one");
            RuleFor(x => x.ShortName)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.TestingAreaId)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
        }
        private bool BeUniqueName(TestDto testDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _testService.IsNameUnique(testDto, LoggedInUserId, IsSuperAdmin);
        }
    }
    public class ImportTestValidator : AbstractValidator<TestDto>
    {
        public ImportTestValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.ShortName)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.TestingAreaId)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
        }
    }




}
