using FluentValidation;
using ForLab.DTO.DiseaseProgram.PatientAssumptionParameter;
using ForLab.DTO.DiseaseProgram.ProductAssumptionParameter;
using ForLab.DTO.DiseaseProgram.Program;
using ForLab.DTO.DiseaseProgram.ProgramTest;
using ForLab.DTO.DiseaseProgram.TestingAssumptionParameter;
using ForLab.Services.DiseaseProgram.Program;
using System.Collections.Generic;
using System.Linq;

namespace ForLab.Validators.Program
{
    public class ProgramValidator : AbstractValidator<ProgramDto>
    {
        readonly IProgramService _programService;
        readonly int LoggedInUserId;
        readonly bool IsSuperAdmin;
        public ProgramValidator(IProgramService programService, int loggedInUserId, bool isSuperAdmin)
        {
            _programService = programService;
            LoggedInUserId = loggedInUserId;
            IsSuperAdmin = isSuperAdmin;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Program name is required.")
                .NotNull()
                .Must(BeUniqueName).WithMessage("The program name is already exist please try a new one");
            RuleFor(x => x.DiseaseId)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.NumberOfYears)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.ProgramTestDtos)
                .Must(BeUniquePatientGroup).WithMessage("You should not duplicate the 'Patient Group' the same 'Test")
                .Must(BeUniqueTestingProtocol).WithMessage("You should not duplicate the 'Testing Protocol Name'");
            RuleFor(x => x.PatientAssumptionParameterDtos)
                .Must(BeUniquePatientAssumptionName).WithMessage("You should not duplicate the 'Patient Assumption Name'");
            RuleFor(x => x.TestingAssumptionParameterDtos)
                .Must(BeUniqueTestingAssumptionName).WithMessage("You should not duplicate the 'Testing Assumption Name'");
            RuleFor(x => x.ProductAssumptionParameterDtos)
                .Must(BeUniqueProductAssumptionName).WithMessage("You should not duplicate the 'Product Assumption Name'");
        }
        private bool BeUniqueName(ProgramDto programDto, string newValue)
        {
            if (string.IsNullOrEmpty(newValue)) return true;
            return _programService.IsNameUnique(programDto, LoggedInUserId, IsSuperAdmin);
        }
        private bool BeUniquePatientGroup(ProgramDto programDto, List<ProgramTestDto> newValue)
        {
            var duplicates = programDto.ProgramTestDtos
                                    .Select(x => x.TestingProtocolDto)
                                    .Where(x => !x.IsDeleted)
                                    .GroupBy(x => new 
                                    {
                                        PatientGroupId = x.PatientGroupId,
                                        TestId = x.TestId
                                    })
                                    .Any(g => g.Count() > 1);
            if (duplicates)
            {
                return false;
            }
            return true;
        }
        private bool BeUniqueTestingProtocol(ProgramDto programDto, List<ProgramTestDto> newValue)
        {
            var duplicates = programDto.ProgramTestDtos
                                    .Select(x => x.TestingProtocolDto)
                                    .Where(x => !x.IsDeleted)
                                    .GroupBy(x => x.Name)
                                    .Any(g => g.Count() > 1);
            if (duplicates)
            {
                return false;
            }
            return true;
        }
        private bool BeUniquePatientAssumptionName(ProgramDto programDto, List<PatientAssumptionParameterDto> newValue)
        {
            var duplicates = programDto.PatientAssumptionParameterDtos
                                            .Where(x => !x.IsDeleted)
                                            .GroupBy(x => x.Name.Trim().ToLower())
                                            .Any(g => g.Count() > 1);
            if (duplicates)
            {
                return false;
            }
            return true;
        }
        private bool BeUniqueTestingAssumptionName(ProgramDto programDto, List<TestingAssumptionParameterDto> newValue)
        {
            var duplicates = programDto.TestingAssumptionParameterDtos
                                            .Where(x => !x.IsDeleted)
                                            .GroupBy(x => x.Name.Trim().ToLower())
                                            .Any(g => g.Count() > 1);
            if (duplicates)
            {
                return false;
            }
            return true;
        }
        private bool BeUniqueProductAssumptionName(ProgramDto programDto, List<ProductAssumptionParameterDto> newValue)
        {
            var duplicates = programDto.ProductAssumptionParameterDtos
                                            .Where(x => !x.IsDeleted)
                                            .GroupBy(x => x.Name.Trim().ToLower())
                                            .Any(g => g.Count() > 1);
            if (duplicates)
            {
                return false;
            }
            return true;
        }
    }



    public class ImportProgramValidator : AbstractValidator<ProgramDto>
    {
        public ImportProgramValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.DiseaseId)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.NumberOfYears)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();
        }
    }
}
