using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastTest
{
    public class ForecastTestFilterDto : BaseFilterDto
    {
        public int ForecastInfoId { get; set; }
        public int TestId { get; set; }
        public string Name { get; set; }
    }
}
