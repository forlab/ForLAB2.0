using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastMorbidityProgram;
using ForLab.Services.Forecasting.ForecastMorbidityProgram;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastMorbidityProgramsController : BaseController
    {
        private readonly IForecastMorbidityProgramService _forecastMorbidityProgramService;

        public ForecastMorbidityProgramsController(
            IForecastMorbidityProgramService forecastMorbidityProgramService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastMorbidityProgramService = forecastMorbidityProgramService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastMorbidityProgramFilterDto filterDto)
        {
            _response = _forecastMorbidityProgramService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }

        [HttpPost]
        public IActionResult ExportForecastMorbidityPrograms(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastMorbidityProgramFilterDto filterDto = null)
        {
            var file = _forecastMorbidityProgramService.ExportForecastMorbidityProgram(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}
