using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Product.Instrument;
using ForLab.Repositories.Product.Instrument;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Product.Instrument
{
    public interface IInstrumentService : IGService<InstrumentDto, Data.DbModels.ProductSchema.Instrument, IInstrumentRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, InstrumentFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(InstrumentFilterDto filterDto = null);
        Task<IResponseDTO> GetInstrumentDetails(int instrumentId);
        Task<IResponseDTO> CreateInstrument(InstrumentDto instrumentDto);
        Task<IResponseDTO> UpdateInstrument(InstrumentDto instrumentDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int instrumentId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActiveForSelected(List<int> ids, bool isActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateSharedForSelected(List<int> ids, bool shared, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveInstrument(int instrumentId, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> ImportInstruments(List<InstrumentDto> instrumentDtos, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportInstruments(int? pageIndex = null, int? pageSize = null, InstrumentFilterDto filterDto = null);
        // Validators methods
        bool IsNameUnique(InstrumentDto instrumentDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> IsUsed(int instrumentId);
    }
}
