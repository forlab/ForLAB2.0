using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastTestingAssumptionValue
{
    public class ForecastTestingAssumptionValueDto : BaseEntityDto
    {
        public int TestingAssumptionParameterId { get; set; }
        public int ForecastInfoId { get; set; }
        public decimal? Value { get; set; }

        // UI
        public string ForecastInfoName { get; set; }
        public string TestingAssumptionParameterName { get; set; }
    }
}
