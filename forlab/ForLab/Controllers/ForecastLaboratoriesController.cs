using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastLaboratory;
using ForLab.Services.Forecasting.ForecastLaboratory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastLaboratoriesController : BaseController
    {
        private readonly IForecastLaboratoryService _forecastLaboratoryService;

        public ForecastLaboratoriesController(
            IForecastLaboratoryService forecastLaboratoryService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastLaboratoryService = forecastLaboratoryService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastLaboratoryFilterDto filterDto)
        {
            _response = _forecastLaboratoryService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportForecastLaboratories(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastLaboratoryFilterDto filterDto = null)
        {
            var file = _forecastLaboratoryService.ExportForecastLaboratory(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}
