using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastProductAssumptionValue
{
    public class ForecastProductAssumptionValueDto : BaseEntityDto
    {
        public int ProductAssumptionParameterId { get; set; }
        public int ForecastInfoId { get; set; }
        public decimal? Value { get; set; }

        // UI
        public string ForecastInfoName { get; set; }
        public string ProductAssumptionParameterName { get; set; }
    }
}
