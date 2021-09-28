using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Product.LaboratoryProductPrice;
using ForLab.Repositories.Product.LaboratoryProductPrice;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Product.LaboratoryProductPrice
{
    public interface ILaboratoryProductPriceService : IGService<LaboratoryProductPriceDto, Data.DbModels.ProductSchema.LaboratoryProductPrice, ILaboratoryProductPriceRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, LaboratoryProductPriceFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(LaboratoryProductPriceFilterDto filterDto = null);
        Task<IResponseDTO> GetLaboratoryProductPriceDetails(int laboratoryProductPriceId);
        Task<IResponseDTO> CreateLaboratoryProductPrice(LaboratoryProductPriceDto laboratoryProductPriceDto);
        Task<IResponseDTO> UpdateLaboratoryProductPrice(LaboratoryProductPriceDto laboratoryProductPriceDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int laboratoryProductPriceId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveLaboratoryProductPrice(int laboratoryProductPriceId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportLaboratoryProductPrices(int? pageIndex = null, int? pageSize = null, LaboratoryProductPriceFilterDto filterDto = null);
        Task<IResponseDTO> ImportLaboratoryProductPrices(List<LaboratoryProductPriceDto> laboratoryProductPriceDtos, int LoggedInUserId, bool IsSuperAdmin);

        // Validators methods
        bool IsLaboratoryProductUnique(LaboratoryProductPriceDto laboratoryProductPriceDto);
    }
}
