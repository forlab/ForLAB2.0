using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Product.ProductUsage;
using ForLab.Repositories.Product.ProductUsage;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Product.ProductUsage
{
    public interface IProductUsageService : IGService<ProductUsageDto, Data.DbModels.ProductSchema.ProductUsage, IProductUsageRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ProductUsageFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(ProductUsageFilterDto filterDto = null);
        Task<IResponseDTO> GetProductUsageDetails(int productUsageId);
        Task<IResponseDTO> CreateProductUsage(ProductUsageDto productUsageDto);
        Task<IResponseDTO> UpdateProductUsage(ProductUsageDto productUsageDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int productUsageId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveProductUsage(int productUsageId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportProductUsages(int? pageIndex = null, int? pageSize = null, ProductUsageFilterDto filterDto = null);
        Task<IResponseDTO> ImportProductUsages(List<ProductUsageDto> productUsageDtos, bool isProduct, int LoggedInUserId, bool IsSuperAdmin);
        // Validators methods
        bool IsProductUsagePeriodUnique(ProductUsageDto productUsageDto);
        bool IsProductUsageInstrumentUnique(ProductUsageDto productUsageDto);
        bool IsTestUsageUnique(ProductUsageDto productUsageDto);
    }
}
