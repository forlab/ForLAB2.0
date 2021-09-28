using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastLaboratory;
using ForLab.Repositories.Forecasting.ForecastLaboratory;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastLaboratory
{
    public interface IForecastLaboratoryService : IGService<ForecastLaboratoryDto, Data.DbModels.ForecastingSchema.ForecastLaboratory, IForecastLaboratoryRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastLaboratoryFilterDto filterDto = null);
        GeneratedFile ExportForecastLaboratory(int? pageIndex = null, int? pageSize = null, ForecastLaboratoryFilterDto filterDto = null);
    }
}
