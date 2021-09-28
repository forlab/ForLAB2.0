using System;

namespace ForLab.DTO.Forecasting.ForecastMorbidityTargetBase
{
    public class ExportForecastMorbidityTargetBaseDto
    {
        // this should be used if morbidity is target based
        public string ForecastInfoName { get; set; }
        public string ForecastLaboratoryLaboratoryName { get; set; }
        public string ForecastMorbidityProgramName { get; set; }
        public decimal CurrentPatient { get; set; }
        public decimal TargetPatient { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
