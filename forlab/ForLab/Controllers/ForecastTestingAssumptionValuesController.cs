using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastTestingAssumptionValue;
using ForLab.Services.Forecasting.ForecastTestingAssumptionValue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastTestingAssumptionValuesController : BaseController
    {
        private readonly IForecastTestingAssumptionValueService _forecastTestingAssumptionValueService;

        public ForecastTestingAssumptionValuesController(
            IForecastTestingAssumptionValueService forecastTestingAssumptionValueService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastTestingAssumptionValueService = forecastTestingAssumptionValueService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastTestingAssumptionValueFilterDto filterDto)
        {
            _response = _forecastTestingAssumptionValueService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }

        [HttpPost]
        public IActionResult ExportForecastTestingAssumptionValues(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastTestingAssumptionValueFilterDto filterDto = null)
        {
            var file = _forecastTestingAssumptionValueService.ExportForecastTestingAssumptionValue(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}
