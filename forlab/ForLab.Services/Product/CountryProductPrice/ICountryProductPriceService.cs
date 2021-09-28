using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Product.CountryProductPrice;
using ForLab.Repositories.Product.CountryProductPrice;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Product.CountryProductPrice
{
    public interface ICountryProductPriceService : IGService<CountryProductPriceDto, Data.DbModels.ProductSchema.CountryProductPrice, ICountryProductPriceRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, CountryProductPriceFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(CountryProductPriceFilterDto filterDto = null);
        Task<IResponseDTO> GetCountryProductPriceDetails(int countryProductPriceId);
        Task<IResponseDTO> CreateCountryProductPrice(CountryProductPriceDto countryProductPriceDto);
        Task<IResponseDTO> UpdateCountryProductPrice(CountryProductPriceDto countryProductPriceDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int countryProductPriceId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> RemoveCountryProductPrice(int countryProductPriceId, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportCountryProductPrices(int? pageIndex = null, int? pageSize = null, CountryProductPriceFilterDto filterDto = null);
        Task<IResponseDTO> ImportCountryProductPrices(List<CountryProductPriceDto> countryProductPriceDtos, int LoggedInUserId, bool IsSuperAdmin);
        // Validators methods
        bool IsCountryProductUnique(CountryProductPriceDto countryProductPriceDto);
    }
}
