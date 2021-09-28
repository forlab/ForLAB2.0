using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastMorbidityTargetBase;
using ForLab.Services.Forecasting.ForecastMorbidityTargetBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastMorbidityTargetBasesController : BaseController
    {
        private readonly IForecastMorbidityTargetBaseService _forecastMorbidityTargetBaseService;

        public ForecastMorbidityTargetBasesController(
            IForecastMorbidityTargetBaseService forecastMorbidityTargetBaseService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastMorbidityTargetBaseService = forecastMorbidityTargetBaseService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastMorbidityTargetBaseFilterDto filterDto)
        {
            _response = _forecastMorbidityTargetBaseService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportForecastMorbidityTargetBases(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastMorbidityTargetBaseFilterDto filterDto = null)
        {
            var file = _forecastMorbidityTargetBaseService.ExportForecastMorbidityTargetBase(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [HttpGet]
        public IResponseDTO ForecastMorbidityTargetBasesChart(int forecastInfoId, int? pageIndex, int? pageSize)
        {
            _response = _forecastMorbidityTargetBaseService.ForecastMorbidityTargetBasesChart(forecastInfoId, pageIndex, pageSize);
            return _response;
        }
    }
}
