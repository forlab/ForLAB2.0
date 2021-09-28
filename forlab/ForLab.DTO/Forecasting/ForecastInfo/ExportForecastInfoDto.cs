using System;

namespace ForLab.DTO.Forecasting.ForecastInfo
{
    public class ExportForecastInfoDto
    {
        public string ForecastInfoLevelName { get; set; }
        public string CountryName { get; set; }
        public string ForecastMethodologyName { get; set; }
        public string ForecastInfoStatusName { get; set; }
        public string ScopeOfTheForecastName { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public decimal WastageRate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Creator { get; set; }
    }
}
