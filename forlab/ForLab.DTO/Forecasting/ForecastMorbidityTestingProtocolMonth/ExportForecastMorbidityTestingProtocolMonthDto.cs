using System;

namespace ForLab.DTO.Forecasting.ForecastMorbidityTestingProtocolMonth
{
    public class ExportForecastMorbidityTestingProtocolMonthDto
    {
        public string ForecastInfoName { get; set; }
        public string ProgramName { get; set; }
        public string TestName { get; set; }
        public string PatientGroupName { get; set; }
        public string CalculationPeriodMonthName { get; set; }
        public decimal? Value { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
