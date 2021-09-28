using ForLab.Core.Interfaces;
using ForLab.DTO.Dashboard;
using ForLab.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace ForLab.API.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(
            IDashboardService dashboardService,
            IResponseDTO response,
            IHttpContextAccessor httpContextAccessor) : base(response, httpContextAccessor)
        {
            _dashboardService = dashboardService;
        }

        #region Main Dashboard
        [HttpGet]
        public IResponseDTO MainCardCounts()
        {
            _response = _dashboardService.MainCardCounts();
            return _response;
        }


        [HttpGet]
        public IResponseDTO NumberOfLaboratories(int? countryId = null, int? regionId = null)
        {
            _response = _dashboardService.NumberOfLaboratories(countryId, regionId);
            return _response;
        }


        [HttpGet]
        public IResponseDTO NumberOfDiseases(int? countryId = null)
        {
            _response = _dashboardService.NumberOfDiseases(countryId);
            return _response;
        }

        [HttpGet]
        public IResponseDTO InquiryQuestionsChart(int numOfMonths)
        {
            _response = _dashboardService.InquiryQuestionsChart(numOfMonths);
            return _response;
        }

        [HttpGet]
        public IResponseDTO UsersChart(int numOfMonths)
        {
            _response = _dashboardService.UsersChart(numOfMonths);
            return _response;
        }

        [HttpGet]
        public IResponseDTO LaboratoriesChart(int numOfMonths, int? countryId = null, int? regionId = null)
        {
            _response = _dashboardService.LaboratoriesChart(numOfMonths, countryId, regionId);
            return _response;
        }
        #endregion


        #region Forecast Dashboard
        [HttpGet]
        public async Task<IResponseDTO> ForecastCardCounts()
        {
            _response = await _dashboardService.ForecastCardCounts();
            return _response;
        }

        [HttpGet]
        public IResponseDTO ForecastsChart(int numOfMonths, int? countryPeriodId = null)
        {
            _response = _dashboardService.ForecastsChart(numOfMonths, countryPeriodId);
            return _response;
        }

        [HttpGet]
        public IResponseDTO NumberOfForecasts([FromQuery] DashboardNumberOfForecastsFilterDto filterDto)
        {
            _response = _dashboardService.NumberOfForecasts(filterDto);
            return _response;
        }
        #endregion
    }
}
