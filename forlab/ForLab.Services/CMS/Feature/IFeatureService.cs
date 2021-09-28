using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.Feature;
using ForLab.Repositories.CMS.Feature;
using ForLab.Services.Generics;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.CMS.Feature
{
    public interface IFeatureService : IGService<FeatureDto, Data.DbModels.CMSSchema.Feature, IFeatureRepository>
    {
        IResponseDTO GetAll(string rootPath, int? pageIndex = null, int? pageSize = null, FeatureFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(FeatureFilterDto filterDto = null);
        Task<IResponseDTO> GetFeatureDetails(string rootPath, int featureId);
        Task<IResponseDTO> CreateFeature(FeatureDto featureDto, IFormFile file = null);
        Task<IResponseDTO> UpdateFeature(FeatureDto featureDto, IFormFile file = null);
        Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int featureId, bool IsActive);
        Task<IResponseDTO> RemoveFeature(int featureId, int loggedInUserId);

        // Validators methods
        bool IsTitleUnique(FeatureDto featureDto);
    }
}
