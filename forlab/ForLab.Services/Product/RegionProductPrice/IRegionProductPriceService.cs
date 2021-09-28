using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Product.RegionProductPrice;
using ForLab.Repositories.Product.RegionProductPrice;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Product.RegionProductPrice
{
    public interface IRegionProductPriceService : IGService<RegionProductPriceDto, Data.DbModels.ProductSchema.RegionProductPrice, IRegionProductPriceRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, RegionProductPriceFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(RegionProductPriceFilterDto filterDto = null);
        Task<IResponseDTO> GetRegionProductPriceDetails(int regionProductPriceId);
        Task<IResponseDTO> CreateRegionProductPrice(RegionProductPriceDto regionProductPriceDto);
        Task<IResponseDTO> UpdateRegionProductPrice(RegionProductPriceDto regionProductPriceDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int regionProductPriceId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveRegionProductPrice(int regionProductPriceId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportRegionProductPrices(int? pageIndex = null, int? pageSize = null, RegionProductPriceFilterDto filterDto = null);
        Task<IResponseDTO> ImportRegionProductPrices(List<RegionProductPriceDto> regionProductPriceDtos, int LoggedInUserId, bool IsSuperAdmin);

        // Validators methods
        bool IsRegionProductUnique(RegionProductPriceDto regionProductPriceDto);
    }
}
