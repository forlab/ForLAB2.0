using ForLab.DTO.Common;

namespace ForLab.DTO.Dashboard
{
    public class DashboardNumberOfForecastsFilterDto : BaseFilterDto
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public int? CountryId { get; set; }
        public int? RegionId { get; set; }
        public string Name { get; set; }
    }

    public class DashboardNumberOfForecastsDto
    {
        public int NumberOfForecasts { get; set; }
        public string Name { get; set; }
    }
}
