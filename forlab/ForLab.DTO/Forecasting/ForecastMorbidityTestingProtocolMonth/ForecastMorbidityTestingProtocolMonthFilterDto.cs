using ForLab.DTO.Common;

namespace ForLab.DTO.Forecasting.ForecastMorbidityTestingProtocolMonth
{
    public class ForecastMorbidityTestingProtocolMonthFilterDto : BaseFilterDto
    {
        public int ForecastInfoId { get; set; }
        public int ProgramId { get; set; } // adding group by accordions
        public int TestId { get; set; } // display drop down lists with tests under this program from programtest table
        public int PatientGroupId { get; set; } // display dropdown with all patient groups for the selected program
        public int TestingProtocolId { get; set; } // select testing protocols based on test id and patient group id should be in testing protocol patient group for this protocol
        public int CalculationPeriodMonthId { get; set; } // based on the selected testing protocol then fetch all months (M1,M2...ETC)
        public decimal? Value { get; set; }
        public string Name { get; set; }
    }
}
