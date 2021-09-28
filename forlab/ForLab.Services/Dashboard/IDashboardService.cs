using ForLab.Core.Interfaces;
using ForLab.DTO.Dashboard;
using System.Threading.Tasks;

namespace ForLab.Services.Dashboard
{
    public interface IDashboardService
    {
        #region Main Dashboard
        IResponseDTO MainCardCounts();
        IResponseDTO NumberOfLaboratories(int? countryId = null, int? regionId = null);
        IResponseDTO NumberOfDiseases(int? countryId = null);
        IResponseDTO InquiryQuestionsChart(int numOfMonths);
        IResponseDTO UsersChart(int numOfMonths);
        IResponseDTO LaboratoriesChart(int numOfMonths, int? countryId = null, int? regionId = null);
        #endregion

        #region Forecast Dashboard
        Task<IResponseDTO> ForecastCardCounts();
        IResponseDTO ForecastsChart(int numOfMonths, int? countryPeriodI = null);
        IResponseDTO NumberOfForecasts(DashboardNumberOfForecastsFilterDto filterDto);
        #endregion
    }
}
