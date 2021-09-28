using ForLab.Core.Interfaces;
using ForLab.DTO.Forecasting.ForecastCategory;
using ForLab.Services.Forecasting.ForecastCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForLab.API.Controllers
{
    [Authorize]
    public class ForecastCategoriesController : BaseController
    {
        private readonly IForecastCategoryService _forecastCategoryService;

        public ForecastCategoriesController(
            IForecastCategoryService forecastCategoryService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _forecastCategoryService = forecastCategoryService;
        }


        [HttpGet]
        public IResponseDTO GetAll(int? pageIndex, int? pageSize, [FromQuery] ForecastCategoryFilterDto filterDto)
        {
            _response = _forecastCategoryService.GetAll(pageIndex, pageSize, filterDto);
            return _response;
        }

        [HttpPost]
        public IActionResult ExportForecastCategories(int? pageIndex = null, int? pageSize = null, [FromQuery] ForecastCategoryFilterDto filterDto = null)
        {
            var file = _forecastCategoryService.ExportForecastCategory(pageIndex, pageSize, filterDto);
            return File((byte[])file.Content, file.Extension, file.Name);
        }

    }
}
