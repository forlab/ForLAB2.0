using ForLab.Core.Common;
using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastPatientGroup;
using ForLab.Repositories.Forecasting.ForecastPatientGroup;
using ForLab.Services.Generics;

namespace ForLab.Services.Forecasting.ForecastPatientGroup
{
    public interface IForecastPatientGroupService : IGService<ForecastPatientGroupDto, Data.DbModels.ForecastingSchema.ForecastPatientGroup, IForecastPatientGroupRepository>
    {
        IResponseDTO GetAll(int? pageIndex = null, int? pageSize = null, ForecastPatientGroupFilterDto filterDto = null);
        GeneratedFile ExportForecastPatientGroup(int? pageIndex = null, int? pageSize = null, ForecastPatientGroupFilterDto filterDto = null);
    }
}
