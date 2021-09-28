using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastLaboratoryTestService;
using ForLab.Repositories.Forecasting.ForecastLaboratoryTestService;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastLaboratoryTestService
{
    public interface IForecastLaboratoryTestServiceService : IGService<ForecastLaboratoryTestServiceDto, Data.DbModels.ForecastingSchema.ForecastLaboratoryTestService, IForecastLaboratoryTestServiceRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastLaboratoryTestServiceFilterDto filterDto = null);
        GeneratedFile ExportForecastLaboratoryTestService(int? pageIndex = null, int? pageSize = null, ForecastLaboratoryTestServiceFilterDto filterDto = null);
        IResponseDTO ForecastLaboratoryTestServicesChart(int forecastInfoId, int? pageIndex = null, int? pageSize = null);
    }
}
