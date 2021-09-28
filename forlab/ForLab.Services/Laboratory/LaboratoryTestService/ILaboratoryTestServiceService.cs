using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Laboratory.LaboratoryTestService;
using ForLab.Repositories.Laboratory.LaboratoryTestService;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Laboratory.LaboratoryTestService
{
    public interface ILaboratoryTestService : IGService<LaboratoryTestServiceDto, Data.DbModels.LaboratorySchema.LaboratoryTestService, ILaboratoryTestServiceRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryTestServiceFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(LaboratoryTestServiceFilterDto filterDto = null);
        Task<IResponseDTO> GetLaboratoryTestServiceDetails(int laboratoryTestServiceId);
        Task<IResponseDTO> CreateLaboratoryTestService(LaboratoryTestServiceDto laboratoryTestServiceDto);
        Task<IResponseDTO> UpdateLaboratoryTestService(LaboratoryTestServiceDto laboratoryTestServiceDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int laboratoryTestServiceId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveLaboratoryTestService(int laboratoryTestServiceId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportLaboratoryTestServices(int? pageIndex = null, int? pageSize = null, LaboratoryTestServiceFilterDto filterDto = null);
        Task<IResponseDTO> ImportLaboratoryTestServices(List<LaboratoryTestServiceDto> laboratoryTestServiceDtos, int LoggedInUserId, bool IsSuperAdmin);

        // Validators methods
        bool IsTestServiceUnique(LaboratoryTestServiceDto laboratoryTestServiceDto);
        Task<IResponseDTO> IsUsed(int laboratoryTestServiceDto);
    }
}
