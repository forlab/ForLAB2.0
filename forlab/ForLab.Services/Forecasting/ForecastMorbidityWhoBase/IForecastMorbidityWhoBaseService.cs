using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastMorbidityWhoBase;
using ForLab.Repositories.Forecasting.ForecastMorbidityWhoBase;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastMorbidityWhoBase
{
    public interface IForecastMorbidityWhoBaseService : IGService<ForecastMorbidityWhoBaseDto, Data.DbModels.ForecastingSchema.ForecastMorbidityWhoBase, IForecastMorbidityWhoBaseRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastMorbidityWhoBaseFilterDto filterDto = null);
        GeneratedFile ExportForecastMorbidityWhoBase(int? pageIndex = null, int? pageSize = null, ForecastMorbidityWhoBaseFilterDto filterDto = null);
        IResponseDTO ForecastMorbidityWhoBasesChart(int forecastInfoId, int? pageIndex = null, int? pageSize = null);
    }
}
