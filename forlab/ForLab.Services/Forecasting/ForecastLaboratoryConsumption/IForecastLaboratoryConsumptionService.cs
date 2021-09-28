using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastLaboratoryConsumption;
using ForLab.Repositories.Forecasting.ForecastLaboratoryConsumption;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastLaboratoryConsumption
{
    public interface IForecastLaboratoryConsumptionService : IGService<ForecastLaboratoryConsumptionDto, Data.DbModels.ForecastingSchema.ForecastLaboratoryConsumption, IForecastLaboratoryConsumptionRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastLaboratoryConsumptionFilterDto filterDto = null);
        GeneratedFile ExportForecastLaboratoryConsumption(int? pageIndex = null, int? pageSize = null, ForecastLaboratoryConsumptionFilterDto filterDto = null);
        IResponseDTO ForecastLaboratoryConsumptionsChart(int forecastInfoId, int? pageIndex = null, int? pageSize = null);
    }
}
