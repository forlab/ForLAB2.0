using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastInstrument
{
    public class ForecastInstrumentDto : BaseEntityDto
    {
        public int InstrumentId { get; set; }
        public int ForecastInfoId { get; set; }

        // UI
        public string InstrumentName { get; set; }
        public string InstrumentTestingAreaName { get; set; }
        public string ForecastInfoName { get; set; }
    }
}
