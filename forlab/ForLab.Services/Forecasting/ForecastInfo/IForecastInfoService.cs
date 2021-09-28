using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastInfo;
using ForLab.Repositories.Forecasting.ForecastInfo;
using ForLab.Services.Generics;
using System.Threading.Tasks;

namespace ForLab.Services.Forecasting.ForecastInfo
{
    public interface IForecastInfoService : IGService<ForecastInfoDto, Data.DbModels.ForecastingSchema.ForecastInfo, IForecastInfoRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastInfoFilterDto filterDto = null);
        IResponseDTO GetAllAsDrp(ForecastInfoFilterDto filterDto = null);
        Task<IResponseDTO> GetForecastInfoDetails(int forecastInfoId);
        Task<IResponseDTO> GetForecastInfoDetailsForUpdate(int forecastInfoId, int loggedInUserId);
        Task<IResponseDTO> CreateForecastInfo(ForecastInfoDto forecastInfoDto);
        Task<IResponseDTO> UpdateForecastInfo(ForecastInfoDto forecastInfoDto, int LoggedInUserId, bool IsSuperAdmin);
        Task<IResponseDTO> UpdateIsActive(int forecastInfoId, bool IsActive, int LoggedInUserId, bool IsSuperAdmin);
        GeneratedFile ExportForecastInfo(int? pageIndex = null, int? pageSize = null, ForecastInfoFilterDto filterDto = null);
        // Validators methods
        bool IsNameUnique(ForecastInfoDto forecastInfoDto, int LoggedInUserId, bool IsSuperAdmin);
    }
}
