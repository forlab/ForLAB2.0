using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Laboratory.LaboratoryWorkingDay;
using ForLab.Repositories.Laboratory.LaboratoryWorkingDay;
using ForLab.Services.Generics;
using System.Threading.Tasks;

namespace ForLab.Services.Laboratory.LaboratoryWorkingDay
{
    public interface ILaboratoryWorkingDayService : IGService<LaboratoryWorkingDayDto, Data.DbModels.LaboratorySchema.LaboratoryWorkingDay, ILaboratoryWorkingDayRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryWorkingDayFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(LaboratoryWorkingDayFilterDto filterDto = null);
        Task<IResponseDTO> GetLaboratoryWorkingDayDetails(int laboratoryWorkingDayId);
        Task<IResponseDTO> CreateLaboratoryWorkingDay(LaboratoryWorkingDayDto laboratoryWorkingDayDto);
        Task<IResponseDTO> UpdateLaboratoryWorkingDay(LaboratoryWorkingDayDto laboratoryWorkingDayDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int laboratoryWorkingDayId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveLaboratoryWorkingDay(int laboratoryWorkingDayId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportLaboratoryWorkingDays(int? pageIndex = null, int? pageSize = null, LaboratoryWorkingDayFilterDto filterDto = null);
        // Validators methods
        bool IsDayUnique(LaboratoryWorkingDayDto laboratoryWorkingDayDto);
        Task<IResponseDTO> IsUsed(int laboratoryWorkingDayDto);

    }
}
