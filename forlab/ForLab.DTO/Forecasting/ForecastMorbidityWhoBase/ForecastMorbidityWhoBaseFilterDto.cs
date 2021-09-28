using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastMorbidityWhoBase
{
    public class ForecastMorbidityWhoBaseFilterDto : BaseFilterDto
    {
        public int ForecastInfoId { get; set; }
        public int DiseaseId { get; set; }
        public int CountryId { get; set; }
        public string Period { get; set; }
        public decimal Count { get; set; }
        public string Name { get; set; }
    }
}
