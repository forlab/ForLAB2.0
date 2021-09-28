using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Disease.CountryDiseaseIncident;
using ForLab.Repositories.Disease.CountryDiseaseIncident;
using ForLab.Services.Generics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForLab.Services.Disease.CountryDiseaseIncident
{
    public interface ICountryDiseaseIncidentService : IGService<CountryDiseaseIncidentDto, Data.DbModels.DiseaseSchema.CountryDiseaseIncident, ICountryDiseaseIncidentRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, CountryDiseaseIncidentFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(CountryDiseaseIncidentFilterDto filterDto = null);
        Task<IResponseDTO> GetCountryDiseaseIncidentDetails(int countryDiseaseIncidentId);
        GeneratedFile ExportCountryDiseaseIncidents(int? pageIndex = null, int? pageSize = null, CountryDiseaseIncidentFilterDto filterDto = null);
        Task<IResponseDTO> CreateCountryDiseaseIncident(CountryDiseaseIncidentDto countryDiseaseIncidentDto);
        Task<IResponseDTO> UpdateCountryDiseaseIncident(CountryDiseaseIncidentDto countryDiseaseIncidentDto);
        Task<IResponseDTO> UpdateIsActive(int loggedInUserId, int countryDiseaseIncidentId, bool IsActive);
        Task<IResponseDTO> UpdateIsActiveForSelected(int loggedInUserId, List<int> ids, bool isActive);
        Task<IResponseDTO> RemoveCountryDiseaseIncident(int countryDiseaseIncidentId, int loggedInUserId);
        Task<IResponseDTO> ImportCountryDiseaseIncidents(List<CountryDiseaseIncidentDto> countryDiseaseIncidentDtos);
        // Validators methods
        bool IsIncidentUnique(CountryDiseaseIncidentDto countryDiseaseIncidentDto);
    }
}