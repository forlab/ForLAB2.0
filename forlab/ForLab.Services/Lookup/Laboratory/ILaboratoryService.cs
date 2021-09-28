using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.Laboratory;
using ForLab.Repositories.Lookup.Laboratory;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Lookup.Laboratory
{
    public interface ILaboratoryService : IGService<LaboratoryDto, Data.DbModels.LookupSchema.Laboratory, ILaboratoryRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(LaboratoryFilterDto filterDto = null);
        Task<IResponseDTO> GetLaboratoryDetails(int laboratoryId);
        Task<IResponseDTO> CreateLaboratory(LaboratoryDto laboratoryDto);
        Task<IResponseDTO> UpdateLaboratory(LaboratoryDto laboratoryDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int laboratoryId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActiveForSelected(List<int> ids, bool isActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateSharedForSelected(List<int> ids, bool shared, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveLaboratory(int laboratoryId, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> ImportLaboratories(List<LaboratoryDto> laboratoryDtos, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportLaboratories(int? pageIndex = null, int? pageSize = null, LaboratoryFilterDto filterDto = null);
        // Validators methods
        bool IsNameUnique(LaboratoryDto laboratoryDto, int LoggedInUserId, bool IsSuperAdmin);
        bool IsLatlngUnique(LaboratoryDto laboratoryDto);
        Task<IResponseDTO> IsUsed(int laboratoryId);
    }
}
