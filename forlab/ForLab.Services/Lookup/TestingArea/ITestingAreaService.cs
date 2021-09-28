using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.TestingArea;
using ForLab.Repositories.Lookup.TestingArea;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Lookup.TestingArea
{
    public interface ITestingAreaService : IGService<TestingAreaDto, Data.DbModels.LookupSchema.TestingArea, ITestingAreaRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, TestingAreaFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(TestingAreaFilterDto filterDto = null);
        Task<IResponseDTO> GetTestingAreaDetails(int testingAreaId);
        Task<IResponseDTO> CreateTestingArea(TestingAreaDto testingAreaDto);
        Task<IResponseDTO> UpdateTestingArea(TestingAreaDto testingAreaDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int testingAreaId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActiveForSelected(List<int> ids, bool isActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateSharedForSelected(List<int> ids, bool shared, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveTestingArea(int testingAreaId, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> ImportTestingAreas(List<TestingAreaDto> testingAreaDtos, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportTestingAreas(int? pageIndex = null, int? pageSize = null, TestingAreaFilterDto filterDto = null);
        // Validators methods
        bool IsNameUnique(TestingAreaDto testingAreaDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> IsUsed(int testingAreaId);
    }
}
