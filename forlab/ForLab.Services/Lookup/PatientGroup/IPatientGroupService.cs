using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.PatientGroup;
using ForLab.Repositories.Lookup.PatientGroup;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Lookup.PatientGroup
{
    public interface IPatientGroupService : IGService<PatientGroupDto, Data.DbModels.LookupSchema.PatientGroup, IPatientGroupRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, PatientGroupFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(PatientGroupFilterDto filterDto = null);
        Task<IResponseDTO> GetPatientGroupDetails(int patientGroupId);
        Task<IResponseDTO> CreatePatientGroup(PatientGroupDto patientGroupDto);
        Task<IResponseDTO> UpdatePatientGroup(PatientGroupDto patientGroupDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int patientGroupId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActiveForSelected(List<int> ids, bool isActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateSharedForSelected(List<int> ids, bool shared, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemovePatientGroup(int patientGroupId, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> ImportPatientGroups(List<PatientGroupDto> patientGroupDtos, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportPatientGroups(int? pageIndex = null, int? pageSize = null, PatientGroupFilterDto filterDto = null);

        // Validators methods
        bool IsNameUnique(PatientGroupDto patientGroupDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> IsUsed(int patientGroupId);
    }
}
