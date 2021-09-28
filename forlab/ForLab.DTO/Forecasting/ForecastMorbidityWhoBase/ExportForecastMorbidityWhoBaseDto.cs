using System;

namespace ForLab.DTO.Forecasting.ForecastMorbidityWhoBase
{
    public class ExportForecastMorbidityWhoBaseDto
    {
        public string ForecastInfoName { get; set; }
        public string DiseaseName { get; set; }
        public string CountryName { get; set; }
        public string Period { get; set; }
        public decimal Count { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
