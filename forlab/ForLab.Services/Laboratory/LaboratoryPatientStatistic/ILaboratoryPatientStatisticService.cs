using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Laboratory.LaboratoryPatientStatistic;
using ForLab.Repositories.Laboratory.LaboratoryPatientStatistic;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Laboratory.LaboratoryPatientStatistic
{
    public interface ILaboratoryPatientStatisticService : IGService<LaboratoryPatientStatisticDto, Data.DbModels.LaboratorySchema.LaboratoryPatientStatistic, ILaboratoryPatientStatisticRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryPatientStatisticFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(LaboratoryPatientStatisticFilterDto filterDto = null);
        Task<IResponseDTO> GetLaboratoryPatientStatisticDetails(int laboratoryPatientStatisticId);
        Task<IResponseDTO> CreateLaboratoryPatientStatistic(LaboratoryPatientStatisticDto laboratoryPatientStatisticDto);
        Task<IResponseDTO> UpdateLaboratoryPatientStatistic(LaboratoryPatientStatisticDto laboratoryPatientStatisticDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int laboratoryPatientStatisticId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveLaboratoryPatientStatistic(int laboratoryPatientStatisticId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportLaboratoryPatientStatistics(int? pageIndex = null, int? pageSize = null, LaboratoryPatientStatisticFilterDto filterDto = null);
        Task<IResponseDTO> ImportLaboratoryPatientStatistics(List<LaboratoryPatientStatisticDto> laboratoryPatientStatisticDtos, int LoggedInUserId, bool IsSuperAdmin);

        // Validators methods
        bool IsPatientStatisticUnique(LaboratoryPatientStatisticDto laboratoryPatientStatisticDto);
        Task<IResponseDTO> IsUsed(int laboratoryPatientStatisticDto);
    }
}
