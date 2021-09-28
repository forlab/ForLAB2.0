using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastResult;
using ForLab.Services.Forecasting.ForecastResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastResultsController : BaseController
    {
        private readonly IForecastResultService _forecastResultService;

        public ForecastResultsController(
            IForecastResultService forecastResultService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastResultService = forecastResultService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastResultFilterDto filterDto)
        {
            _response = _forecastResultService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportForecastResults(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastResultFilterDto filterDto = null)
        {
            var file = _forecastResultService.ExportForecastResult(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [HttpGet]
        public IResponseDTO ForecastResultsChart(int forecastInfoId, int? pageIndex, int? pageSize)
        {
            _response = _forecastResultService.ForecastResultsChart(forecastInfoId, pageIndex, pageSize);
            return _response;
        }
    }
}
