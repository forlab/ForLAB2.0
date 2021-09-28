using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastPatientAssumptionValue;
using ForLab.Repositories.Forecasting.ForecastPatientAssumptionValue;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastPatientAssumptionValue
{
    public interface IForecastPatientAssumptionValueService : IGService<ForecastPatientAssumptionValueDto, Data.DbModels.ForecastingSchema.ForecastPatientAssumptionValue, IForecastPatientAssumptionValueRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastPatientAssumptionValueFilterDto filterDto = null);
        GeneratedFile ExportForecastPatientAssumptionValue(int? pageIndex = null, int? pageSize = null, ForecastPatientAssumptionValueFilterDto filterDto = null);
    }
}
