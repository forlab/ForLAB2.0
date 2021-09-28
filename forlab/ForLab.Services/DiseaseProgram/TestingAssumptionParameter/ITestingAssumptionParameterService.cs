using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.DiseaseProgram.TestingAssumptionParameter;
using ForLab.Repositories.DiseaseProgram.TestingAssumptionParameter;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.DiseaseProgram.TestingAssumptionParameter
{
    public interface ITestingAssumptionParameterService : IGService<TestingAssumptionParameterDto, Data.DbModels.DiseaseProgramSchema.TestingAssumptionParameter, ITestingAssumptionParameterRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, TestingAssumptionParameterFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(TestingAssumptionParameterFilterDto filterDto = null);
        IResponseDTO GetAllTestingAssumptionsForForcast(string programIds);
        Task<IResponseDTO> GetTestingAssumptionParameterDetails(int testingAssumptionParameterId);
        Task<IResponseDTO> CreateTestingAssumptionParameter(TestingAssumptionParameterDto testingAssumptionParameterDto);
        Task<IResponseDTO> UpdateTestingAssumptionParameter(TestingAssumptionParameterDto testingAssumptionParameterDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int testingAssumptionParameterId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveTestingAssumptionParameter(int testingAssumptionParameterId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportTestingAssumptionParameters(int? pageIndex = null, int? pageSize = null, TestingAssumptionParameterFilterDto filterDto = null);
        Task<IResponseDTO> ImportTestingAssumptionParameters(List<TestingAssumptionParameterDto> testingAssumptionParameterDtos, int LoggedInUserId, bool IsSuperAdmin);
        // Validators methods
        bool IsNameUnique(TestingAssumptionParameterDto testingAssumptionParameterDto);
        Task<IResponseDTO> IsUsed(int testingAssumptionParameterDto);
    }
}
