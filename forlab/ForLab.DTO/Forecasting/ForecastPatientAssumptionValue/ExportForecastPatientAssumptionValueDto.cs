using System;

namespace ForLab.DTO.Forecasting.ForecastPatientAssumptionValue
{
    public class ExportForecastPatientAssumptionValueDto
    {
        public string ForecastInfoName { get; set; }
        public string PatientAssumptionParameterName { get; set; }
        public decimal? Value { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
