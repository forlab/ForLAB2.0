using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastPatientGroup;
using ForLab.Services.Forecasting.ForecastPatientGroup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastPatientGroupsController : BaseController
    {
        private readonly IForecastPatientGroupService _forecastPatientGroupService;

        public ForecastPatientGroupsController(
            IForecastPatientGroupService forecastPatientGroupService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastPatientGroupService = forecastPatientGroupService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastPatientGroupFilterDto filterDto)
        {
            _response = _forecastPatientGroupService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }


        [HttpPost]
        public IActionResult ExportForecastPatientGroups(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastPatientGroupFilterDto filterDto = null)
        {
            var file = _forecastPatientGroupService.ExportForecastPatientGroup(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }
    }
}
