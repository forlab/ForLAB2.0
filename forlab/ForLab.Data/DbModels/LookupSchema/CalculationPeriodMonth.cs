using ForLab.Data.BaseModeling;
using ForLab.Data.DbModels.ForecastingSchema;
using ForLab.Data.DbModels.TestingSchema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForLab.Data.DbModels.LookupSchema
{
    // set M1 M2 M3 ... M12 FOR ONE YEAR. 
    // SET M1 M2 M3 .... M24 FOR TWO YEARS
    [Table("CalculationPeriodMonths", Schema = "Lookup")]
    public class CalculationPeriodMonth : StaticLookup
    {
        public CalculationPeriodMonth()
        {
            TestingProtocolCalculationPeriodMonths = new HashSet<TestingProtocolCalculationPeriodMonth>();
            ForecastMorbidityTestingProtocolMonths = new HashSet<ForecastMorbidityTestingProtocolMonth>();
        }
        public int CalculationPeriodId { get; set; }
        public virtual CalculationPeriod CalculationPeriod { get; set; }
        public virtual ICollection<TestingProtocolCalculationPeriodMonth> TestingProtocolCalculationPeriodMonths { get; set; }
        public virtual ICollection<ForecastMorbidityTestingProtocolMonth> ForecastMorbidityTestingProtocolMonths { get; set; }
    }
}
