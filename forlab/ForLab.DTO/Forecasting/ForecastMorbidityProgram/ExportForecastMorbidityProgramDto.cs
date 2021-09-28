using System;

namespace ForLab.DTO.Forecasting.ForecastMorbidityProgram
{
    public class ExportForecastMorbidityProgramDto
    {
        public string ForecastInfoName { get; set; }
        public string ProgramName { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
