using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastMorbidityTestingProtocolMonth;
using ForLab.Services.Forecasting.ForecastMorbidityTestingProtocolMonth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastMorbidityTestingProtocolMonthsController : BaseController
    {
        private readonly IForecastMorbidityTestingProtocolMonthService _forecastMorbidityTestingProtocolMonthService;

        public ForecastMorbidityTestingProtocolMonthsController(
            IForecastMorbidityTestingProtocolMonthService forecastMorbidityTestingProtocolMonthService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastMorbidityTestingProtocolMonthService = forecastMorbidityTestingProtocolMonthService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastMorbidityTestingProtocolMonthFilterDto filterDto)
        {
            _response = _forecastMorbidityTestingProtocolMonthService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }

        [HttpPost]
        public IActionResult ExportForecastMorbidityTestingProtocolMonths(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastMorbidityTestingProtocolMonthFilterDto filterDto = null)
        {
            var file = _forecastMorbidityTestingProtocolMonthService.ExportForecastMorbidityTestingProtocolMonth(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}
