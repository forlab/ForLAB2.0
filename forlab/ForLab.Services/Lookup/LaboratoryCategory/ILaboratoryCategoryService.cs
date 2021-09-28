using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.LaboratoryCategory;
using ForLab.Repositories.Lookup.LaboratoryCategory;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Lookup.LaboratoryCategory
{
    public interface ILaboratoryCategoryService : IGService<LaboratoryCategoryDto, Data.DbModels.LookupSchema.LaboratoryCategory, ILaboratoryCategoryRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryCategoryFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(LaboratoryCategoryFilterDto filterDto = null);
        Task<IResponseDTO> GetLaboratoryCategoryDetails(int laboratoryCategoryId);
        Task<IResponseDTO> CreateLaboratoryCategory(LaboratoryCategoryDto laboratoryCategoryDto);
        Task<IResponseDTO> UpdateLaboratoryCategory(LaboratoryCategoryDto laboratoryCategoryDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int laboratoryCategoryId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActiveForSelected(List<int> ids, bool isActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateSharedForSelected(List<int> ids, bool shared, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveLaboratoryCategory(int laboratoryCategoryId, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> ImportLaboratoryCategories(List<LaboratoryCategoryDto> laboratoryCategoryDtos, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportLaboratoryCategories(int? pageIndex = null, int? pageSize = null, LaboratoryCategoryFilterDto filterDto = null);
        // Validators methods
        bool IsNameUnique(LaboratoryCategoryDto laboratoryCategoryDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> IsUsed(int laboratoryCategoryId);
    }
}
