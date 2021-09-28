using ForLab.Core.Interfaces;
using ForLab.DTO.CMS.UsefulResource;
using ForLab.Repositories.CMS.UsefulResource;
using ForLab.Services.Generics;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.CMS.UsefulResource
{
    public interface IUsefulResourceService : IGService<UsefulResourceDto, Data.DbModels.CMSSchema.UsefulResource, IUsefulResourceRepository>
    {
        IResponseDTO GetAll(string rootPath, int? pageIndex = null, int? pageSize = null, UsefulResourceFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(UsefulResourceFilterDto filterDto = null);
        Task<IResponseDTO> GetUsefulResourceDetails(int usefulResourceId);
        Task<IResponseDTO> CreateUsefulResources(List<UsefulResourceDto> usefulResourceDtos, List<IFormFile> files = null);
        Task<IResponseDTO> CreateUsefulResource(UsefulResourceDto usefulResourceDto, IFormFile file = null);
        Task<IResponseDTO> UpdateUsefulResource(UsefulResourceDto usefulResourceDto, IFormFile file = null);
        Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int usefulResourceId, bool IsActive);
        Task<IResponseDTO> RemoveUsefulResource(int usefulResourceId, int loggedInUserId);
        Task<IResponseDTO> IncrementDownloadCount(int usefulResourceId);
        // Validators methods
        bool IsTitleUnique(UsefulResourceDto usefulResourceDto);
        bool IsUrlUnique(UsefulResourceDto usefulResourceDto);
        bool IsAttachmentNameUnique(UsefulResourceDto usefulResourceDto);
    }
}
