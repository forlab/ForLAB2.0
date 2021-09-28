using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastLaboratoryConsumption;
using ForLab.Services.Forecasting.ForecastLaboratoryConsumption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastLaboratoryConsumptionsController : BaseController
    {
        private readonly IForecastLaboratoryConsumptionService _forecastLaboratoryConsumptionService;

        public ForecastLaboratoryConsumptionsController(
            IForecastLaboratoryConsumptionService forecastLaboratoryConsumptionService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastLaboratoryConsumptionService = forecastLaboratoryConsumptionService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastLaboratoryConsumptionFilterDto filterDto)
        {
            _response = _forecastLaboratoryConsumptionService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportForecastLaboratoryConsumptions(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastLaboratoryConsumptionFilterDto filterDto = null)
        {
            var file = _forecastLaboratoryConsumptionService.ExportForecastLaboratoryConsumption(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }


        [HttpGet]
        public IResponseDTO ForecastLaboratoryConsumptionsChart(int forecastInfoId, int? pageIndex, int? pageSize)
        {
            _response = _forecastLaboratoryConsumptionService.ForecastLaboratoryConsumptionsChart(forecastInfoId, pageIndex, pageSize);
            return _response;
        }

    }
}
