using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastTestingAssumptionValue;
using ForLab.Repositories.Forecasting.ForecastTestingAssumptionValue;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastTestingAssumptionValue
{
    public interface IForecastTestingAssumptionValueService : IGService<ForecastTestingAssumptionValueDto, Data.DbModels.ForecastingSchema.ForecastTestingAssumptionValue, IForecastTestingAssumptionValueRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastTestingAssumptionValueFilterDto filterDto = null);
        GeneratedFile ExportForecastTestingAssumptionValue(int? pageIndex = null, int? pageSize = null, ForecastTestingAssumptionValueFilterDto filterDto = null);
    }
}
