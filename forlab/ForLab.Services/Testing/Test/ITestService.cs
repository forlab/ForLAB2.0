using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Testing.Test;
using ForLab.Repositories.Testing.Test;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Testing.Test
{
    public interface ITestService : IGService<TestDto, Data.DbModels.TestingSchema.Test, ITestRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, TestFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(TestFilterDto filterDto = null);
        Task<IResponseDTO> GetTestDetails(int testId);
        Task<IResponseDTO> CreateTest(TestDto testDto);
        Task<IResponseDTO> UpdateTest(TestDto testDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int testId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActiveForSelected(List<int> ids, bool isActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateSharedForSelected(List<int> ids, bool shared, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveTest(int testId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportTests(int? pageIndex = null, int? pageSize = null, TestFilterDto filterDto = null);
        Task<IResponseDTO> ImportTests(List<TestDto> testDtos, int LoggedInUserId, bool IsSuperAdmin);

        // Validators methods
        bool IsNameUnique(TestDto testDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> IsUsed(int testDto);
    }
}
