using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Laboratory.LaboratoryConsumption;
using ForLab.Repositories.Laboratory.LaboratoryConsumption;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Laboratory.LaboratoryConsumption
{
    public interface ILaboratoryConsumptionService : IGService<LaboratoryConsumptionDto, Data.DbModels.LaboratorySchema.LaboratoryConsumption, ILaboratoryConsumptionRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryConsumptionFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(LaboratoryConsumptionFilterDto filterDto = null);
        Task<IResponseDTO> GetLaboratoryConsumptionDetails(int laboratoryConsumptionId);
        Task<IResponseDTO> CreateLaboratoryConsumption(LaboratoryConsumptionDto laboratoryConsumptionDto);
        Task<IResponseDTO> UpdateLaboratoryConsumption(LaboratoryConsumptionDto laboratoryConsumptionDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int laboratoryConsumptionId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveLaboratoryConsumption(int laboratoryConsumptionId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportLaboratoryConsumptions(int? pageIndex = null, int? pageSize = null, LaboratoryConsumptionFilterDto filterDto = null);
        Task<IResponseDTO> ImportLaboratoryConsumptions(List<LaboratoryConsumptionDto> laboratoryConsumptionDtos, int LoggedInUserId, bool IsSuperAdmin);
        // Validators methods
        bool IsConsumptionUnique(LaboratoryConsumptionDto laboratoryConsumptionDto);
        Task<IResponseDTO> IsUsed(int laboratoryConsumptionDto);
    }
}
