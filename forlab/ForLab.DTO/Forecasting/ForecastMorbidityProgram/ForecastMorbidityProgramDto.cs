using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastMorbidityProgram
{
    public class ForecastMorbidityProgramDto : BaseEntityDto
    {
        public int ForecastInfoId { get; set; }
        public int ProgramId { get; set; }

        //UI
        public string ForecastInfoName { get; set; }
        public string ProgramName { get; set; }
    }
}
