using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastInstrument
{
    public class ForecastInstrumentFilterDto : BaseFilterDto
    {
        public int InstrumentId { get; set; }
        public int ForecastInfoId { get; set; }
        public string Name { get; set; }
    }
}
