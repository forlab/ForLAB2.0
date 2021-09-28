using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastCategory
{
    public class ForecastCategoryFilterDto : BaseFilterDto
    {
        public int ForecastInfoId { get; set; }
        public string Name { get; set; }
    }
}
