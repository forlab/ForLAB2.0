using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastTest;
using ForLab.Repositories.Forecasting.ForecastTest;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastTest
{
    public interface IForecastTestService : IGService<ForecastTestDto, Data.DbModels.ForecastingSchema.ForecastTest, IForecastTestRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastTestFilterDto filterDto = null);
        GeneratedFile ExportForecastTest(int? pageIndex = null, int? pageSize = null, ForecastTestFilterDto filterDto = null);
    }
}
