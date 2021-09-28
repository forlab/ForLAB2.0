using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastMorbidityTestingProtocolMonth;
using ForLab.Repositories.Forecasting.ForecastMorbidityTestingProtocolMonth;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastMorbidityTestingProtocolMonth
{
    public interface IForecastMorbidityTestingProtocolMonthService : IGService<ForecastMorbidityTestingProtocolMonthDto, Data.DbModels.ForecastingSchema.ForecastMorbidityTestingProtocolMonth, IForecastMorbidityTestingProtocolMonthRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastMorbidityTestingProtocolMonthFilterDto filterDto = null);
        GeneratedFile ExportForecastMorbidityTestingProtocolMonth(int? pageIndex = null, int? pageSize = null, ForecastMorbidityTestingProtocolMonthFilterDto filterDto = null);
    }
}
