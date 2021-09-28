using System;

namespace ForLab.DTO.Forecasting.ForecastPatientGroup
{
    public class ExportForecastPatientGroupDto
    {
        public string PatientGroupName { get; set; }
        public string ProgramName { get; set; }
        public string ForecastinfoName { get; set; }
        public decimal Percentage { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
