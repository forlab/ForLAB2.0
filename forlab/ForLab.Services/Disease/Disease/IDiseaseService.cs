using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Disease.Disease;
using ForLab.Repositories.Disease.Disease;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Disease.Disease
{
    public interface IDiseaseService : IGService<DiseaseDto, Data.DbModels.DiseaseSchema.Disease, IDiseaseRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, DiseaseFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(DiseaseFilterDto filterDto = null);
        Task<IResponseDTO> GetDiseaseDetails(int diseaseId);
        GeneratedFile ExportDiseases(int? pageIndex = null, int? pageSize = null, DiseaseFilterDto filterDto = null);
        Task<IResponseDTO> CreateDisease(DiseaseDto diseaseDto);
        Task<IResponseDTO> UpdateDisease(DiseaseDto diseaseDto);
        Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int diseaseId, bool IsActive);
        Task<IResponseDTO> UpdateIsActiveForSelected(int loggedInUserId, List<int> ids, bool isActive);
        Task<IResponseDTO> RemoveDisease(int diseaseId, int loggedInUserId);
        Task<IResponseDTO> ImportDiseases(List<DiseaseDto> diseaseDtos);

        // Validators methods
        bool IsNameUnique(DiseaseDto diseaseDto);
        Task<IResponseDTO> IsUsed(int diseaseDto);
    }
}

