using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastProductAssumptionValue
{
    public class ForecastProductAssumptionValueFilterDto : BaseFilterDto
    {
        public int ProductAssumptionParameterId { get; set; }
        public int ForecastInfoId { get; set; }
        public decimal? Value { get; set; }
        public string Name { get; set; }
    }
}
