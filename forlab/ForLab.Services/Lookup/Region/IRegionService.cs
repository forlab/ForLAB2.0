using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.Region;
using ForLab.Repositories.Lookup.Region;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Lookup.Region
{
    public interface IRegionService : IGService<RegionDto, Data.DbModels.LookupSchema.Region, IRegionRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, RegionFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(RegionFilterDto filterDto = null);
        Task<IResponseDTO> GetRegionDetails(int regionId);
        Task<IResponseDTO> CreateRegion(RegionDto regionDto);
        Task<IResponseDTO> UpdateRegion(RegionDto regionDto);
        Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int regionId, bool IsActive);
        Task<IResponseDTO> UpdateIsActiveForSelected(int loggedInUserId, List<int> ids, bool isActive);
        Task<IResponseDTO> RemoveRegion(int regionId, int loggedInUserId);
        Task<IResponseDTO> ImportRegions(List<RegionDto> regionDtos);
        GeneratedFile ExportRegions(int? pageIndex = null, int? pageSize = null, RegionFilterDto filterDto = null);
        // Validators methods
        bool IsNameUnique(RegionDto regionDto);
        Task<IResponseDTO> IsUsed(int regionId);
    }
}
