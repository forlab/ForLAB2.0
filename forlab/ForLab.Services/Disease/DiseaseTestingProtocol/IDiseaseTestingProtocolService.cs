using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Disease.DiseaseTestingProtocol;
using ForLab.Repositories.Disease.DiseaseTestingProtocol;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Disease.DiseaseTestingProtocol
{
    public interface IDiseaseTestingProtocolService : IGService<DiseaseTestingProtocolDto, Data.DbModels.DiseaseSchema.DiseaseTestingProtocol, IDiseaseTestingProtocolRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, DiseaseTestingProtocolFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(DiseaseTestingProtocolFilterDto filterDto = null);
        Task<IResponseDTO> GetDiseaseTestingProtocolDetails(int diseaseTestingProtocolId);
        Task<IResponseDTO> CreateDiseaseTestingProtocol(DiseaseTestingProtocolDto diseaseTestingProtocolDto);
        Task<IResponseDTO> UpdateDiseaseTestingProtocol(DiseaseTestingProtocolDto diseaseTestingProtocolDto);
        Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int diseaseTestingProtocolId, bool IsActive);
        Task<IResponseDTO> RemoveDiseaseTestingProtocol(int diseaseTestingProtocolId, int loggedInUserId);
        GeneratedFile ExportDiseaseTestingProtocols(int? pageIndex = null, int? pageSize = null, DiseaseTestingProtocolFilterDto filterDto = null);
        Task<IResponseDTO> ImportDiseaseTestingProtocols(List<DiseaseTestingProtocolDto> diseaseTestingProtocolDtos);

    }
}