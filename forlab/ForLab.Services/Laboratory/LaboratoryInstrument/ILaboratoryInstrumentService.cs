using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Laboratory.LaboratoryInstrument;
using ForLab.Repositories.Laboratory.LaboratoryInstrument;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Laboratory.LaboratoryInstrument
{
    public interface ILaboratoryInstrumentService : IGService<LaboratoryInstrumentDto, Data.DbModels.LaboratorySchema.LaboratoryInstrument, ILaboratoryInstrumentRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryInstrumentFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(LaboratoryInstrumentFilterDto filterDto = null);
        Task<IResponseDTO> GetLaboratoryInstrumentDetails(int laboratoryInstrumentId);
        Task<IResponseDTO> CreateLaboratoryInstrument(LaboratoryInstrumentDto laboratoryInstrumentDto);
        Task<IResponseDTO> UpdateLaboratoryInstrument(LaboratoryInstrumentDto laboratoryInstrumentDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int laboratoryInstrumentId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveLaboratoryInstrument(int laboratoryInstrumentId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportLaboratoryInstruments(int? pageIndex = null, int? pageSize = null, LaboratoryInstrumentFilterDto filterDto = null);
        Task<IResponseDTO> ImportLaboratoryInstruments(List<LaboratoryInstrumentDto> laboratoryInstrumentDtos, int LoggedInUserId, bool IsSuperAdmin);

        // Validators methods
        bool IsLaboratoryInstrumentUnique(LaboratoryInstrumentDto laboratoryInstrumentDto);
        bool IsValidPercentage(LaboratoryInstrumentDto laboratoryInstrumentDto);
        Task<IResponseDTO> IsUsed(int laboratoryInstrumentDto);
    }
}
