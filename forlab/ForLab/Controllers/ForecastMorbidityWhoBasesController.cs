using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastMorbidityWhoBase;
using ForLab.Services.Forecasting.ForecastMorbidityWhoBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastMorbidityWhoBasesController : BaseController
    {
        private readonly IForecastMorbidityWhoBaseService _forecastMorbidityWhoBaseService;

        public ForecastMorbidityWhoBasesController(
            IForecastMorbidityWhoBaseService forecastMorbidityWhoBaseService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastMorbidityWhoBaseService = forecastMorbidityWhoBaseService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastMorbidityWhoBaseFilterDto filterDto)
        {
            _response = _forecastMorbidityWhoBaseService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportForecastMorbidityWhoBases(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastMorbidityWhoBaseFilterDto filterDto = null)
        {
            var file = _forecastMorbidityWhoBaseService.ExportForecastMorbidityWhoBase(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [HttpGet]
        public IResponseDTO ForecastMorbidityWhoBasesChart(int forecastInfoId, int? pageIndex, int? pageSize)
        {
            _response = _forecastMorbidityWhoBaseService.ForecastMorbidityWhoBasesChart(forecastInfoId, pageIndex, pageSize);
            return _response;
        }
    }
}
