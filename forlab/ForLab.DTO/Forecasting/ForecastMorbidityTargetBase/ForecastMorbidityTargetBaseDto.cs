using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastMorbidityTargetBase
{
    public class ForecastMorbidityTargetBaseDto : BaseEntityDto
    {
        // this should be used if morbidity is target based
        public int ForecastInfoId { get; set; }
        public int ForecastLaboratoryId { get; set; }
        public int ForecastMorbidityProgramId { get; set; }
        public decimal CurrentPatient { get; set; }
        public decimal TargetPatient { get; set; }

        // UI
        public string ForecastLaboratoryLaboratoryName { get; set; }
        public string ForecastMorbidityProgramProgramName { get; set; }
    }
}
