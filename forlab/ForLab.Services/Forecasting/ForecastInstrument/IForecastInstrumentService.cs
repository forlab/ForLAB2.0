using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastInstrument;
using ForLab.Repositories.Forecasting.ForecastInstrument;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastInstrument
{
    public interface IForecastInstrumentService : IGService<ForecastInstrumentDto, Data.DbModels.ForecastingSchema.ForecastInstrument, IForecastInstrumentRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastInstrumentFilterDto filterDto = null);
        GeneratedFile ExportForecastInstrument(int? pageIndex = null, int? pageSize = null, ForecastInstrumentFilterDto filterDto = null);
    }
}
