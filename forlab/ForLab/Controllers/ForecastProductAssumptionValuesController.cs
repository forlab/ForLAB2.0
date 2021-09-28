using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastProductAssumptionValue;
using ForLab.Services.Forecasting.ForecastProductAssumptionValue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastProductAssumptionValuesController : BaseController
    {
        private readonly IForecastProductAssumptionValueService _forecastProductAssumptionValueService;

        public ForecastProductAssumptionValuesController(
            IForecastProductAssumptionValueService forecastProductAssumptionValueService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastProductAssumptionValueService = forecastProductAssumptionValueService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastProductAssumptionValueFilterDto filterDto)
        {
            _response = _forecastProductAssumptionValueService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportForecastProductAssumptionValues(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastProductAssumptionValueFilterDto filterDto = null)
        {
            var file = _forecastProductAssumptionValueService.ExportForecastProductAssumptionValue(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}
