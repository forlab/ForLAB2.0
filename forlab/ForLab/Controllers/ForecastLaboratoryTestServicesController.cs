using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastLaboratoryTestService;
using ForLab.Services.Forecasting.ForecastLaboratoryTestService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastLaboratoryTestServicesController : BaseController
    {
        private readonly IForecastLaboratoryTestServiceService _forecastLaboratoryTestServiceService;

        public ForecastLaboratoryTestServicesController(
            IForecastLaboratoryTestServiceService forecastLaboratoryTestServiceService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastLaboratoryTestServiceService = forecastLaboratoryTestServiceService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastLaboratoryTestServiceFilterDto filterDto)
        {
            _response = _forecastLaboratoryTestServiceService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }

        [HttpPost]
        public IActionResult ExportForecastLaboratoryTestServices(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastLaboratoryTestServiceFilterDto filterDto = null)
        {
            var file = _forecastLaboratoryTestServiceService.ExportForecastLaboratoryTestService(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }



        [HttpGet]
        public IResponseDTO ForecastLaboratoryTestServicesChart(int forecastInfoId, int? pageIndex, int? pageSize)
        {
            _response = _forecastLaboratoryTestServiceService.ForecastLaboratoryTestServicesChart(forecastInfoId, pageIndex, pageSize);
            return _response;
        }
    }
}
