using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastMorbidityTargetBase
{
    public class ForecastMorbidityTargetBaseFilterDto : BaseFilterDto
    {
        public int ForecastInfoId { get; set; }
        public int ForecastLaboratoryId { get; set; }
        public int ForecastMorbidityProgramId { get; set; }
        public decimal? CurrentPatient { get; set; }
        public decimal? TargetPatient { get; set; }
        public string Name { get; set; }
    }
}
