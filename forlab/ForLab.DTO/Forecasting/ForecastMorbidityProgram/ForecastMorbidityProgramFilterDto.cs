using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastMorbidityProgram
{
    public class ForecastMorbidityProgramFilterDto : BaseFilterDto
    {
        public string Name { get; set; }
        public int ForecastInfoId { get; set; }
        public int ProgramId { get; set; }
    }
}
