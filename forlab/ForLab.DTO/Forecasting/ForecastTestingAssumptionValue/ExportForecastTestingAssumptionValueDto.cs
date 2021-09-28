using System;

namespace ForLab.DTO.Forecasting.ForecastTestingAssumptionValue
{
    public class ExportForecastTestingAssumptionValueDto
    {
        public string ForecastInfoName { get; set; }
        public string TestingAssumptionParameterName { get; set; }
        public decimal? Value { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
      
    }
}
