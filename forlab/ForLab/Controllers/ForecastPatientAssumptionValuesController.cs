using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastPatientAssumptionValue;
using ForLab.Services.Forecasting.ForecastPatientAssumptionValue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastPatientAssumptionValuesController : BaseController
    {
        private readonly IForecastPatientAssumptionValueService _forecastPatientAssumptionValueService;

        public ForecastPatientAssumptionValuesController(
            IForecastPatientAssumptionValueService forecastPatientAssumptionValueService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastPatientAssumptionValueService = forecastPatientAssumptionValueService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastPatientAssumptionValueFilterDto filterDto)
        {
            _response = _forecastPatientAssumptionValueService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportForecastPatientAssumptionValues(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastPatientAssumptionValueFilterDto filterDto = null)
        {
            var file = _forecastPatientAssumptionValueService.ExportForecastPatientAssumptionValue(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}
