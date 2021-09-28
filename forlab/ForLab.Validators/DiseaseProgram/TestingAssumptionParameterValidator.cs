using FluentValidation;
using ForLab.DTO.DiseaseProgram.TestingAssumptionParameter;
using ForLab.Services.DiseaseProgram.TestingAssumptionParameter;

namespace ForLab.Validators.TestingAssumptionParameter
{
    public class TestingAssumptionParameterValidator : AbstractValidator<TestingAssumptionParameterDto>
    {
        readonly ITestingAssumptionParameterService _testingAssumptionParameterService;
        public TestingAssumptionParameterValidator(ITestingAssumptionParameterService testingAssumptionParameterService)
        {
            _testingAssumptionParameterService = testingAssumptionParameterService;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .NotNull()
                .Must(BeUniqueName).WithMessage("The name is already exist please try a new one");
            RuleFor(x => x.ProgramId)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
        }
        private bool BeUniqueName(TestingAssumptionParameterDto testingAssumptionParameterDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _testingAssumptionParameterService.IsNameUnique(testingAssumptionParameterDto);
        }
    }
    public class ImportTestingAssumptionParameterValidator : AbstractValidator<TestingAssumptionParameterDto>
    {
        public ImportTestingAssumptionParameterValidator()
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
