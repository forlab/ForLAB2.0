using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastMorbidityProgram;
using ForLab.Repositories.Forecasting.ForecastMorbidityProgram;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastMorbidityProgram
{
    public interface IForecastMorbidityProgramService : IGService<ForecastMorbidityProgramDto, Data.DbModels.ForecastingSchema.ForecastMorbidityProgram, IForecastMorbidityProgramRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastMorbidityProgramFilterDto filterDto = null);
        GeneratedFile ExportForecastMorbidityProgram(int? pageIndex = null, int? pageSize = null, ForecastMorbidityProgramFilterDto filterDto = null);
    }
}
