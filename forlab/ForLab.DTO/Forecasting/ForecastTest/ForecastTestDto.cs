using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastTest
{
    public class ForecastTestDto : BaseEntityDto
    {
        public int ForecastInfoId { get; set; }
        public int TestId { get; set; }

        // UI
        public string ForecastInfoName { get; set; }
        public string TestName { get; set; }
        public string TestTestingAreaName { get; set; }
    }
}
