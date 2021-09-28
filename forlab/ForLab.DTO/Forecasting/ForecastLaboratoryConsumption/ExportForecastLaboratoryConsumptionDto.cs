using System;

namespace ForLab.DTO.Forecasting.ForecastLaboratoryConsumption
{
    public class ExportForecastLaboratoryConsumptionDto
    {
        public string ForecastInfoName { get; set; }
        public string LaboratoryName { get; set; }
        public string ProductName { get; set; }
        public string Period { get; set; }
        public decimal AmountForecasted { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
