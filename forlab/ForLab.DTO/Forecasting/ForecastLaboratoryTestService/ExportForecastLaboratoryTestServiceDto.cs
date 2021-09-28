using System;

namespace ForLab.DTO.Forecasting.ForecastLaboratoryTestService
{
    public class ExportForecastLaboratoryTestServiceDto
    {
        public string ForecastInfoName { get; set; }
        public string LaboratoryName { get; set; }
        public string TestName { get; set; }
        public string Period { get; set; }
        public decimal AmountForecasted { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
