using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastInfo
{
    public class ForecastInfoFilterDto : BaseFilterDto
    {
        public int ForecastInfoLevelId { get; set; }
        public int CountryId { get; set; }
        public int ForecastMethodologyId { get; set; }
        public int ForecastInfoStatusId { get; set; }
        public int ScopeOfTheForecastId { get; set; }
        public string Name { get; set; }
        public bool? IsTargetBased { get; set; } // if morbidity
        public bool? IsAggregate { get; set; }
    }
}
