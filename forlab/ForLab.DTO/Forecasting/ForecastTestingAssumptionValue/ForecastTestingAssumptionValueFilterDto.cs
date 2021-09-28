using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastTestingAssumptionValue
{
    public class ForecastTestingAssumptionValueFilterDto : BaseFilterDto
    {
        public int TestingAssumptionParameterId { get; set; }
        public int ForecastInfoId { get; set; }
        public decimal? Value { get; set; }
        public string Name { get; set; }
    }
}
