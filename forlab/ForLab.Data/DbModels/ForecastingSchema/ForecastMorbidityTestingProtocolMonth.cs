using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.DiseaseProgramSchema;
using ForLab.Data.DbModels.LookupSchema;
using ForLab.Data.DbModels.TestingSchema;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.ForecastingSchema
{
    // if morbidity only
    [Table("ForecastMorbidityTestingProtocolMonths", Schema = "Forecasting")]
    public class ForecastMorbidityTestingProtocolMonth: BaseEntity
    {
        public int ForecastInfoId { get; set; }
        public int ProgramId { get; set; } // adding group by accordions
        public int TestId { get; set; } // display drop down lists with tests under this program from programtest table
        public int PatientGroupId { get; set; } // display dropdown with all patient groups for the selected program
        public int TestingProtocolId { get; set; } // select testing protocols based on test id and patient group id should be in testing protocol patient group for this protocol
        public int CalculationPeriodMonthId { get; set; } // based on the selected testing protocol then fetch all months (M1,M2...ETC)
        public decimal? Value { get; set; } // by default set value as is from metadata , user will be able to change and its optional

        public virtual ForecastInfo ForecastInfo { get; set; }
        public virtual Program Program { get; set; }
        public virtual Test Test { get; set; }
        public virtual PatientGroup PatientGroup { get; set; }
        public virtual CalculationPeriodMonth CalculationPeriodMonth { get; set; }
        public virtual TestingProtocol TestingProtocol { get; set; }
    }
}
