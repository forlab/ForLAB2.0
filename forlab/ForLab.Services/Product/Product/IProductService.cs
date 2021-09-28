using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Product.Product;
using ForLab.Repositories.Product.Product;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Product.Product
{
    public interface IProductService : IGService<ProductDto, Data.DbModels.ProductSchema.Product, IProductRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ProductFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(ProductFilterDto filterDto = null);
        Task<IResponseDTO> GetProductDetails(int productId);
        Task<IResponseDTO> CreateProduct(ProductDto productDto);
        Task<IResponseDTO> UpdateProduct(ProductDto productDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int productId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActiveForSelected(List<int> ids, bool isActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateSharedForSelected(List<int> ids, bool shared, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveProduct(int productId, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> ImportProducts(List<ProductDto> productDtos, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportProducts(int? pageIndex = null, int? pageSize = null, ProductFilterDto filterDto = null);
        // Validators methods
        bool IsNameUnique(ProductDto productDto, int LoggedInUserId, bool IsSuperAdmin);
        bool IsCatalogNoUnique(ProductDto productDto);
        Task<IResponseDTO> IsUsed(int productDto);
    }
}
