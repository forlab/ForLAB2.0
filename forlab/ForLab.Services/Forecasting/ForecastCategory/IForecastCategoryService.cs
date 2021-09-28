using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastCategory;
using ForLab.Repositories.Forecasting.ForecastCategory;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastCategory
{
    public interface IForecastCategoryService : IGService<ForecastCategoryDto, Data.DbModels.ForecastingSchema.ForecastCategory, IForecastCategoryRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastCategoryFilterDto filterDto = null);
        GeneratedFile ExportForecastCategory(int? pageIndex = null, int? pageSize = null, ForecastCategoryFilterDto filterDto = null);
    }
}
