using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Lookup.Country;
using ForLab.Repositories.Lookup.Country;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Lookup.Country
{
    public interface ICountryService : IGService<CountryDto, Data.DbModels.LookupSchema.Country, ICountryRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, CountryFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(CountryFilterDto filterDto = null);
        Task<IResponseDTO> GetCountryDetails(int countryId);
        Task<IResponseDTO> CreateCountry(CountryDto countryDto);
        Task<IResponseDTO> UpdateCountry(CountryDto countryDto);
        Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int countryId, bool IsActive);
        Task<IResponseDTO> UpdateIsActiveForSelected(int loggedInUserId, List<int> ids, bool isActive);
        Task<IResponseDTO> RemoveCountry(int countryId, int loggedInUserId);
        Task<IResponseDTO> ImportCountries(List<CountryDto> countryDtos);
        GeneratedFile ExportCountries(int? pageIndex = null, int? pageSize = null, CountryFilterDto filterDto = null);

        // Validators methods
        bool IsNameUnique(CountryDto countryDto);
        bool IsShortCodeUnique(CountryDto countryDto);
        bool IsLatlngUnique(CountryDto countryDto);
        Task<IResponseDTO> IsUsed(int countryId);
    }
}
