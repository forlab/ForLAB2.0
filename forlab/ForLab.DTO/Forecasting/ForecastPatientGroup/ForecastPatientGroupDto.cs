using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastPatientGroup
{
    public class ForecastPatientGroupDto : BaseEntityDto
    {
        public int ForecastInfoId { get; set; }
        public int PatientGroupId { get; set; }
        public int ProgramId { get; set; }
        public decimal Percentage { get; set; }

        // UI
        public string PatientGroupName { get; set; }
        public string ProgramName { get; set; }
        public string ForecastinfoName { get; set; }
    }
}
