using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastTest;
using ForLab.Services.Forecasting.ForecastTest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastTestsController : BaseController
    {
        private readonly IForecastTestService _forecastTestService;

        public ForecastTestsController(
            IForecastTestService forecastTestService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastTestService = forecastTestService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastTestFilterDto filterDto)
        {
            _response = _forecastTestService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportForecastTests(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastTestFilterDto filterDto = null)
        {
            var file = _forecastTestService.ExportForecastTest(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}
