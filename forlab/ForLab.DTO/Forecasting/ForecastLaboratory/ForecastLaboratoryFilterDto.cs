using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastLaboratory
{
    public class ForecastLaboratoryFilterDto : BaseFilterDto
    {
        public int? ForecastCategoryId { get; set; }
        public int LaboratoryId { get; set; }
        public int ForecastInfoId { get; set; }
        public string Name { get; set; }
    }
}
