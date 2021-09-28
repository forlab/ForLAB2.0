using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastInstrument;
using ForLab.Services.Forecasting.ForecastInstrument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastInstrumentsController : BaseController
    {
        private readonly IForecastInstrumentService _forecastInstrumentService;

        public ForecastInstrumentsController(
            IForecastInstrumentService forecastInstrumentService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastInstrumentService = forecastInstrumentService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastInstrumentFilterDto filterDto)
        {
            _response = _forecastInstrumentService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportForecastInstruments(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastInstrumentFilterDto filterDto = null)
        {
            var file = _forecastInstrumentService.ExportForecastInstrument(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


    }
}
