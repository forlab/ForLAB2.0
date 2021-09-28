using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastMorbidityTargetBase;
using ForLab.Repositories.Forecasting.ForecastMorbidityTargetBase;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastMorbidityTargetBase
{
    public interface IForecastMorbidityTargetBaseService : IGService<ForecastMorbidityTargetBaseDto, Data.DbModels.ForecastingSchema.ForecastMorbidityTargetBase, IForecastMorbidityTargetBaseRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastMorbidityTargetBaseFilterDto filterDto = null);
        GeneratedFile ExportForecastMorbidityTargetBase(int? pageIndex = null, int? pageSize = null, ForecastMorbidityTargetBaseFilterDto filterDto = null);
        IResponseDTO ForecastMorbidityTargetBasesChart(int forecastInfoId, int? pageIndex = null, int? pageSize = null);
    }
}
