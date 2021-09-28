using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastResult;
using ForLab.Repositories.Forecasting.ForecastResult;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastResult
{
    public interface IForecastResultService : IGService<ForecastResultDto, Data.DbModels.ForecastingSchema.ForecastResult, IForecastResultRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastResultFilterDto filterDto = null);
        GeneratedFile ExportForecastResult(int? pageIndex = null, int? pageSize = null, ForecastResultFilterDto filterDto = null);
        IResponseDTO ForecastResultsChart(int forecastInfoId, int? pageIndex = null, int? pageSize = null);
    }
}
