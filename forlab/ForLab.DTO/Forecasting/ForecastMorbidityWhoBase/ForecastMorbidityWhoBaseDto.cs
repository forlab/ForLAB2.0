using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastMorbidityWhoBase
{
    public class ForecastMorbidityWhoBaseDto : BaseEntityDto
    {
        // if forecast morbidity who , bring only previous 5 years
        public int ForecastInfoId { get; set; }
        public int DiseaseId { get; set; }
        public int CountryId { get; set; }
        public string Period { get; set; }
        public decimal Count { get; set; }

        // UI
        public string ForecastInfoName { get; set; }
        public string DiseaseName { get; set; }
        public string CountryName { get; set; }
    }
}
