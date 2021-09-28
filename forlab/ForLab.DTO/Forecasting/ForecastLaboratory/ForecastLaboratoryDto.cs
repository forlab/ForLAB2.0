using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastLaboratory
{
    public class ForecastLaboratoryDto : BaseEntityDto
    {
        public int? ForecastCategoryId { get; set; }
        public int LaboratoryId { get; set; }
        public int ForecastInfoId { get; set; }

        // UI
        public string ForecastCategoryName { get; set; }
        public string LaboratoryName { get; set; }
        public string ForecastInfoName { get; set; }
    }
}
