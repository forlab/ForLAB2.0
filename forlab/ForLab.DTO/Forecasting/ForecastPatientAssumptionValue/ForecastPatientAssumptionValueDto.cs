using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastPatientAssumptionValue
{
    public class ForecastPatientAssumptionValueDto : BaseEntityDto
    {
        public int ForecastInfoId { get; set; }
        public int PatientAssumptionParameterId { get; set; }
        public decimal? Value { get; set; }

        // UI
        public string ForecastInfoName { get; set; }
        public string PatientAssumptionParameterName { get; set; }
    }
}
