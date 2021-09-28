using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.LaboratoryLevel;
using ForLab.Repositories.Lookup.LaboratoryLevel;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Lookup.LaboratoryLevel
{
    public interface ILaboratoryLevelService : IGService<LaboratoryLevelDto, Data.DbModels.LookupSchema.LaboratoryLevel, ILaboratoryLevelRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryLevelFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(LaboratoryLevelFilterDto filterDto = null);
        Task<IResponseDTO> GetLaboratoryLevelDetails(int laboratoryLevelId);
        Task<IResponseDTO> CreateLaboratoryLevel(LaboratoryLevelDto laboratoryLevelDto);
        Task<IResponseDTO> UpdateLaboratoryLevel(LaboratoryLevelDto laboratoryLevelDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int laboratoryLevelId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActiveForSelected(List<int> ids, bool isActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateSharedForSelected(List<int> ids, bool shared, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveLaboratoryLevel(int laboratoryLevelId, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> ImportLaboratoryLevels(List<LaboratoryLevelDto> laboratoryLevelDtos, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportLaboratoryLevels(int? pageIndex = null, int? pageSize = null, LaboratoryLevelFilterDto filterDto = null);
        // Validators methods
        bool IsNameUnique(LaboratoryLevelDto laboratoryLevelDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> IsUsed(int countryId);
    }
}
