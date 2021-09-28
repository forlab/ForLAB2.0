using System;

namespace ForLab.DTO.Forecasting.ForecastProductAssumptionValue
{
    public class ExportForecastProductAssumptionValueDto
    {
        public string ForecastInfoName { get; set; }
        public string ProductAssumptionParameterName { get; set; }
        public decimal Value { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
