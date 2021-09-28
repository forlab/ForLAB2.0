using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastPatientAssumptionValue
{
    public class ForecastPatientAssumptionValueFilterDto : BaseFilterDto
    {
        public int ForecastInfoId { get; set; }
        public int PatientAssumptionParameterId { get; set; }
        public decimal? Value { get; set; }
        public string Name { get; set; }
    }
}
