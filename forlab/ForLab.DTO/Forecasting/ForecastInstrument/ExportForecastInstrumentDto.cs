using System;

namespace ForLab.DTO.Forecasting.ForecastInstrument
{
    public class ExportForecastInstrumentDto 
    {
        public string InstrumentName { get; set; }
        public string ForecastInfoName { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
