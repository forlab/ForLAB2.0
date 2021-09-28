using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastProductAssumptionValue;
using ForLab.Repositories.Forecasting.ForecastProductAssumptionValue;
using ForLab.Services.Generics;
using System.Threading.Tasks;

namespace ForLab.Services.Forecasting.ForecastProductAssumptionValue
{
    public interface IForecastProductAssumptionValueService : IGService<ForecastProductAssumptionValueDto, Data.DbModels.ForecastingSchema.ForecastProductAssumptionValue, IForecastProductAssumptionValueRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastProductAssumptionValueFilterDto filterDto = null);
        GeneratedFile ExportForecastProductAssumptionValue(int? pageIndex = null, int? pageSize = null, ForecastProductAssumptionValueFilterDto filterDto = null);
    }
}
