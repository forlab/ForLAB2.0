using FluentValidation;
using ForLab.DTO.DiseaseProgram.ProgramTest;
using ForLab.Services.DiseaseProgram.ProgramTest;

namespace ForLab.Validators.DiseaseProgram
{
    public class ProgramTestValidator : AbstractValidator<ProgramTestDto>
    {
        readonly IProgramTestService _programTestService;
        public ProgramTestValidator(IProgramTestService programTestService)
        {
            _programTestService = programTestService;

            RuleFor(x => x.TestId)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.TestingProtocolDto.Name)
                 .NotEmpty()
                 .NotNull()
                 .Must(BeUniqueTestingProtocol).WithMessage("Testing Protocol Name is already exist please try a new one");
            RuleFor(x => x.TestingProtocolDto.PatientGroupId)
                 .NotEmpty()
                 .NotNull()
                 .Must(BeUniquePatientGroup).WithMessage("Patient Group is already exist please try a new one");
            // Testin Protocol
            RuleFor(x => x.TestingProtocolDto.CalculationPeriodId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.TestId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.TestingProtocolDto.PatientGroupId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.TestingProtocolDto.TestAfterFirstYear)
               .NotEmpty()
               .NotNull()
               .GreaterThan(0);
            RuleFor(x => x.TestingProtocolDto.BaseLine)
              .NotEmpty()
              .NotNull()
              .GreaterThan(0);
        }

        private bool BeUniqueTestingProtocol(ProgramTestDto programTestDto, string newValue)
        {
            return _programTestService.IsProgramTestTestingProtocolUnique(programTestDto);
        }
        private bool BeUniquePatientGroup(ProgramTestDto programTestDto, int newValue)
        {
            return _programTestService.IsProgramTestPatientGroupUnique(programTestDto);
        }

    }
    public class ImportProgramTestValidator : AbstractValidator<ProgramTestDto>
    {
        public ImportProgramTestValidator()
        {
            RuleFor(x => x.ProgramId)
              .GreaterThan(0)
               .NotEmpty()
               .NotNull();
            RuleFor(x => x.TestingProtocolId)
                 .GreaterThan(0)
                 .NotEmpty()
                 .NotNull();
            RuleFor(x => x.TestId)
                 .GreaterThan(0)
                 .NotEmpty()
                 .NotNull();
        }
    }
}







