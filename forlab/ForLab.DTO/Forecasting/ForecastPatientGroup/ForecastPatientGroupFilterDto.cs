using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastPatientGroup
{
    public class ForecastPatientGroupFilterDto : BaseFilterDto
    {
        public int ForecastInfoId { get; set; }
        public int PatientGroupId { get; set; }
        public int ProgramId { get; set; }
        public decimal? Percentage { get; set; }
        public string Name { get; set; }
    }
}
